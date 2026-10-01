using System.Net;
using System.Text.Json;
using Maros.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Maros.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException appEx)
        {
            _logger.LogWarning(appEx, "Error de negocio en {Path}: {Message}", context.Request.Path, appEx.Message);
            await WriteResponseAsync(context, new ApiErrorResponse(appEx.StatusCode, appEx.Message));
        }
        catch (DbUpdateConcurrencyException dbEx)
        {
            // Genuino conflicto de concurrencia: la fila cambió o desapareció entre la
            // lectura y el guardado, o el UPDATE/XML affected 0 filas. Se separa del
            // catch genérico de abajo porque la causa y la acción correctiva difieren.
            _logger.LogError(dbEx,
                "Conflicto de concurrencia en {Path}. Entidades afectadas: {Entidades}",
                context.Request.Path,
                string.Join(", ", dbEx.Entries.Select(e => e.Metadata.ClrType.Name)));
            await WriteResponseAsync(context, new ApiErrorResponse(
                (int)HttpStatusCode.Conflict,
                "El registro fue modificado por otra petición. Vuelve a cargarlo e inténtalo de nuevo."
            ));
        }
        catch (DbUpdateException dbEx)
        {
            // Captura violaciones de restricciones de SQL Server/PostgreSQL que no anticipamos
            // con un AppException explícito (ej. una condición de carrera en un
            // índice único, o una FK que se nos escapó validar antes de guardar).
            // El detalle real (nombre de restricción, SqlState) solo se registra en el log:
            // nunca se filtra el mensaje SQL crudo al cliente.
            _logger.LogError(dbEx,
                "Violación de integridad en {Path}: {Detalle}",
                context.Request.Path,
                dbEx.InnerException?.Message ?? dbEx.Message);
            await WriteResponseAsync(context, new ApiErrorResponse(
                (int)HttpStatusCode.Conflict,
                "No se pudo completar la operación porque entra en conflicto con datos existentes."
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada procesando {Path}: {Message}", context.Request.Path, ex.Message);
            var errors = new Dictionary<string, string[]>
            {
                ["exception"] = new[] { ex.GetType().Name, ex.Message, ex.StackTrace ?? string.Empty },
                ["innerException"] = ex.InnerException != null ? new[] { ex.InnerException.Message } : Array.Empty<string>()
            };
            await WriteResponseAsync(context, new ApiErrorResponse(
                (int)HttpStatusCode.InternalServerError,
                $"Ocurrió un error inesperado: {ex.Message}",
                errors
            ));
        }
    }

    private static Task WriteResponseAsync(HttpContext context, ApiErrorResponse error)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = error.Status;

        var json = JsonSerializer.Serialize(error, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        return context.Response.WriteAsync(json);
    }
}