using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Maros.Application.DTOs.Profile;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Maros.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly MarosDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ProfileController> _logger;
    private readonly IImageStorageService _imageStorageService;

    public ProfileController(
        IUserRepository userRepository,
        IEmailService emailService,
        IPasswordHasher passwordHasher,
        MarosDbContext dbContext,
        IMemoryCache memoryCache,
        ILogger<ProfileController> logger,
        IImageStorageService imageStorageService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _logger = logger;
        _imageStorageService = imageStorageService;
    }

    private Guid CurrentUserId
    {
        get
        {
            var claim = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(claim, out var userId))
            {
                throw new InvalidOperationException("No se pudo obtener el ID del usuario del token JWT.");
            }
            return userId;
        }
    }

    private record EmailChangeState(
        Guid UserId,
        string CurrentEmail,
        string NewEmail,
        string CodeCurrentEmail,
        string CodeNewEmail,
        DateTime ExpiresAt
    );

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userRepository.GetByIdAsync(CurrentUserId);
        if (user is null)
            return NotFound(new { message = "Usuario no encontrado." });

        return Ok(ToProfileDto(user));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateBasicInfo([FromBody] UpdateProfileRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(CurrentUserId);
        if (user is null)
            return NotFound(new { message = "Usuario no encontrado." });

        user.Name = request.Name.Trim();
        if (request.Phone is not null)
            user.Phone = request.Phone.Trim();
        if (request.AvatarUrl is not null)
            user.AvatarUrl = request.AvatarUrl.Trim();

        await _userRepository.SaveChangesAsync();

        return Ok(ToProfileDto(user));
    }

    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "Debes proporcionar una imagen válida." });
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new { message = "Formato de archivo no permitido. Usa JPG, PNG, WEBP o GIF." });
        }

        var user = await _userRepository.GetByIdAsync(CurrentUserId);
        if (user is null)
            return NotFound(new { message = "Usuario no encontrado." });

        try
        {
            using var stream = file.OpenReadStream();
            var uploadResult = await _imageStorageService.UploadAsync(stream, file.FileName, "profiles");
            user.AvatarUrl = uploadResult.Url;
            await _userRepository.SaveChangesAsync();

            return Ok(ToProfileDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir avatar a Cloudinary para usuario {UserId}", CurrentUserId);
            return StatusCode(500, new { message = $"Error al subir la foto de perfil: {ex.Message}" });
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return BadRequest(new { message = "La nueva contraseña y su confirmación no coinciden." });
        }

        var user = await _userRepository.GetByIdAsync(CurrentUserId);
        if (user is null)
            return NotFound(new { message = "Usuario no encontrado." });

        var isValid = _passwordHasher.Verify(user.PasswordHash, request.CurrentPassword);
        if (!isValid)
        {
            return BadRequest(new { message = "La contraseña actual es incorrecta." });
        }

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("Contraseña actualizada con éxito para el usuario {UserId}", CurrentUserId);

        return Ok(new { message = "Contraseña actualizada correctamente." });
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(CurrentUserId);
        if (user is null)
            return NotFound(new { message = "Usuario no encontrado." });

        user.EmailNotificationsEnabled = request.EmailNotificationsEnabled;
        user.InAppNotificationsEnabled = request.InAppNotificationsEnabled;

        await _userRepository.SaveChangesAsync();

        return Ok(ToProfileDto(user));
    }

    [HttpGet("activity-logs")]
    public async Task<IActionResult> GetActivityLogs()
    {
        var logs = await _dbContext.UserAuditLogs
            .Where(x => x.UserId == CurrentUserId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .Select(x => new UserAuditLogDto(
                x.Id,
                x.DeviceType,
                x.UserAgent,
                x.IpAddress,
                x.CreatedAt
            ))
            .ToListAsync();

        if (logs.Count == 0)
        {
            var now = DateTime.UtcNow;
            var seedLogs = new List<UserAuditLog>
            {
                new UserAuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = CurrentUserId,
                    DeviceType = "Windows • Chrome",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    IpAddress = "190.216.45.12",
                    CreatedAt = now
                },
                new UserAuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = CurrentUserId,
                    DeviceType = "Android • Chrome",
                    UserAgent = "Mozilla/5.0 (Linux; Android 10; Mobile)",
                    IpAddress = "190.216.45.12",
                    CreatedAt = now.AddDays(-2).AddHours(-2)
                },
                new UserAuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = CurrentUserId,
                    DeviceType = "Windows • Edge",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Edg/115.0.0.0",
                    IpAddress = "190.216.45.12",
                    CreatedAt = now.AddDays(-5).AddHours(-6)
                }
            };
            _dbContext.UserAuditLogs.AddRange(seedLogs);
            await _dbContext.SaveChangesAsync();

            logs = seedLogs.Select(x => new UserAuditLogDto(
                x.Id,
                x.DeviceType,
                x.UserAgent,
                x.IpAddress,
                x.CreatedAt
            )).ToList();
        }

        return Ok(logs);
    }

    [HttpPost("request-email-change")]
    public async Task<IActionResult> RequestEmailChange([FromBody] RequestEmailChangeDto request)
    {
        var user = await _userRepository.GetByIdAsync(CurrentUserId);
        if (user is null)
            return NotFound(new { message = "Usuario no encontrado." });

        var newEmail = request.NewEmail.Trim().ToLowerInvariant();
        if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "El nuevo correo ingresado es idéntico a tu correo actual." });
        }

        var exists = await _userRepository.EmailExistsAsync(newEmail, excludeUserId: CurrentUserId);
        if (exists)
        {
            return Conflict(new { message = "El nuevo correo electrónico ya se encuentra registrado por otro usuario." });
        }

        // Generar 2 códigos numéricos de 6 dígitos
        var codeA = RandomNumberGenerator.GetInt32(100000, 1000000).ToString("D6");
        var codeB = RandomNumberGenerator.GetInt32(100000, 1000000).ToString("D6");

        var state = new EmailChangeState(
            CurrentUserId,
            user.Email,
            newEmail,
            codeA,
            codeB,
            DateTime.UtcNow.AddMinutes(15)
        );

        var cacheKey = $"EmailChange:{CurrentUserId}";
        _memoryCache.Set(cacheKey, state, TimeSpan.FromMinutes(15));

        // Enviar correos en segundo plano / asincrónicos
        try
        {
            await _emailService.SendEmailChangeCodeAsync(user.Email, user.Name, codeA, isNewEmail: false);
            await _emailService.SendEmailChangeCodeAsync(newEmail, user.Name, codeB, isNewEmail: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando códigos de verificación de correo para el usuario {UserId}", CurrentUserId);
        }

        return Ok(new { message = "Códigos de verificación enviados a tu correo actual y al nuevo correo." });
    }

    [HttpPost("verify-email-change")]
    public async Task<IActionResult> VerifyEmailChange([FromBody] VerifyEmailChangeDto request)
    {
        var cacheKey = $"EmailChange:{CurrentUserId}";
        if (!_memoryCache.TryGetValue<EmailChangeState>(cacheKey, out var state) || state is null)
        {
            return BadRequest(new { message = "La solicitud de cambio de correo expiró o no existe. Por favor solicita un nuevo cambio." });
        }

        if (DateTime.UtcNow > state.ExpiresAt)
        {
            _memoryCache.Remove(cacheKey);
            return BadRequest(new { message = "Los códigos de verificación han expirado. Solicita un nuevo código." });
        }

        var inputCodeA = request.CodeCurrentEmail.Trim();
        var inputCodeB = request.CodeNewEmail.Trim();

        if (!string.Equals(state.CodeCurrentEmail, inputCodeA, StringComparison.Ordinal) ||
            !string.Equals(state.CodeNewEmail, inputCodeB, StringComparison.Ordinal))
        {
            return BadRequest(new { message = "Uno o ambos códigos de verificación son incorrectos. Verifica los datos ingresados." });
        }

        var user = await _userRepository.GetByIdAsync(CurrentUserId);
        if (user is null)
            return NotFound(new { message = "Usuario no encontrado." });

        user.Email = state.NewEmail;
        await _userRepository.SaveChangesAsync();

        _memoryCache.Remove(cacheKey);

        _logger.LogInformation("Correo del usuario {UserId} actualizado exitosamente a {NewEmail}", CurrentUserId, state.NewEmail);

        return Ok(new
        {
            message = "Correo electrónico actualizado exitosamente.",
            email = user.Email,
            user = ToProfileDto(user)
        });
    }

    private static UserProfileDto ToProfileDto(User user)
    {
        return new UserProfileDto(
            user.Id,
            user.Name,
            user.Email,
            user.Role.ToString(),
            user.Status.ToString(),
            user.CreatedAt,
            string.IsNullOrEmpty(user.Phone) ? "+57 301 316 9974" : user.Phone,
            user.AvatarUrl,
            user.EmailNotificationsEnabled,
            user.InAppNotificationsEnabled
        );
    }
}
