using System.Text;
using Maros.Api.Middleware;
using Maros.Application;
using Maros.Application.Options;
using Maros.Infrastructure;
using Maros.Infrastructure.Persistence;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

using Maros.Api.Converters;

var builder = WebApplication.CreateBuilder(args);

// ─── Servicios base ───────────────────────────────────────────────
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new NullableGuidJsonConverter());
});
builder.Services.AddMemoryCache();

// ─── Timeout extendido para subidas pesadas a Cloudinary (10 min) ──
// El HttpClient por defecto aborta a los 100 s con el error
// "HttpClient.Timeout of 100 seconds elapsing", lo que corta videos .mov de
// iPhone y archivos de alta resolución. CloudinaryImageStorageService consume
// este cliente con nombre para dar margen suficiente a la transcodificación.
builder.Services.AddHttpClient("CloudinaryClient", client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

// ─── Límites de carga de archivos (100 MB) ────────────────────────
// Videos de iPhone (.mov, 10–50 MB), MP3 instrumentales e imágenes.
// Sin este ajuste, Kestrel limita el body a 30 MB y FormOptions puede
// rechazar el multipart con un 400 Bad Request antes de llegar al controller.
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104_857_600; // 100 MB
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104_857_600; // 100 MB
});

// Reemplaza el formato ValidationProblemDetails por defecto de ASP.NET Core
// con nuestro ApiErrorResponse, para que un fallo de [Required]/[EmailAddress]/etc.
// se vea idéntico a un error de negocio (AppException) del lado del cliente.
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                e => e.Key,
                e => e.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
            );

        var response = new Maros.Application.Common.ApiErrorResponse(
            StatusCodes.Status400BadRequest,
            "Uno o más campos no son válidos.",
            errors
        );

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
    };
});
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Maro's Pijamas API",
            Version = "v1",
            Description = "API para el panel administrativo y la landing pública de Maro's Pijamas. " +
                           "Los endpoints bajo /api/public/* no requieren autenticación. " +
                           "El resto requiere un token JWT (Authorization: Bearer <token>).",
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<InvitationOptions>(builder.Configuration.GetSection(InvitationOptions.SectionName));

// ─── CORS ──────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("MarosCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ─── Autenticación JWT ─────────────────────────────────────────────
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey no está configurado en user-secrets.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
    };
});

// ─── Autorización por rol ──────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministrador", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("RequireEditorOrAdmin", policy => policy.RequireRole("Administrador", "Editor"));
});

var app = builder.Build();

// ─── Forwarded Headers (Render / reverse proxy) ────────────────
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    ForwardLimit = null,
    KnownIPNetworks = { },
    KnownProxies = { }
});

// ─── Middleware pipeline ──────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Maro's Pijamas API";
    });
}

// Sembrar/actualizar usuario admin inicial y configuraciones predeterminadas.
// Se ejecuta en todos los entornos para garantizar que el admin exista en producción.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MarosDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.SeedInitialAdminAsync(db, app.Configuration);
    await SiteSettingsSeed.SeedDefaultAsync(db);
    await PageHeaderSeed.SeedDefaultAsync(db);
    await HomeSectionContentSeed.SeedDefaultAsync(db);
}

app.UseHttpsRedirection();
app.UseCors("MarosCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// ─── Health check (Render / monitoreo) ───────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();