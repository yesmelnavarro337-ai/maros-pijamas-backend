using Maros.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Resend;
using System.Net;

namespace Maros.Infrastructure.ExternalServices;

/// <summary>
/// Implementación de <see cref="IEmailService"/> usando el SDK oficial de Resend.
/// Requiere la sección "Resend:ApiKey" configurada en appsettings / variables de entorno.
/// </summary>
public sealed class ResendEmailService : IEmailService
{
    private const string DefaultFromEmail = "onboarding@resend.dev";
    private const string ProjectName = "Maro's Pijamas";
    private const string DefaultFromName  = "Maro's Pijamas - Panel Administrativo";

    private readonly IResend  _resend;
    private readonly string   _fromEmail;
    private readonly string   _fromName;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(
        IResend resend,
        IConfiguration configuration,
        ILogger<ResendEmailService> logger)
    {
        _resend    = resend;
        _logger    = logger;

        var section = configuration.GetSection("Resend");
        _fromEmail  = section["FromEmail"] ?? DefaultFromEmail;
        _fromName   = section["FromName"]  ?? DefaultFromName;
    }

    /// <inheritdoc/>
    public async Task SendInvitationAsync(string to, string name, string acceptUrl)
    {
        _logger.LogInformation("Enviando correo de invitación a {To}", to);

        var message = new EmailMessage
        {
            From    = $"{_fromName} <{_fromEmail}>",
            Subject = "Activa tu cuenta del panel administrativo de Maro's Pijamas",
            HtmlBody = BuildInvitationHtml(name, acceptUrl),
        };
        message.To.Add(to);

        try
        {
            await _resend.EmailSendAsync(message);
            _logger.LogInformation("Correo de invitación enviado a {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al enviar correo de invitación a {To}. Enlace de aceptación: {AcceptUrl}",
                to, acceptUrl);
            // No relanzamos: la invitación ya quedó persistida como Pendiente.
        }
    }

    /// <inheritdoc/>
    public async Task SendEmailChangeCodeAsync(string to, string name, string code, bool isNewEmail)
    {
        _logger.LogInformation("Enviando código de verificación a {To} (isNewEmail={IsNewEmail})", to, isNewEmail);

        var subject = isNewEmail
            ? "Código de verificación de nuevo correo — Maro's Pijamas"
            : "Código de seguridad para cambio de correo — Maro's Pijamas";

        var message = new EmailMessage
        {
            From     = $"{_fromName} <{_fromEmail}>",
            Subject  = subject,
            HtmlBody = BuildCodeHtml(name, code, isNewEmail),
        };
        message.To.Add(to);

        try
        {
            await _resend.EmailSendAsync(message);
            _logger.LogInformation("Código {Code} enviado a {To}", code, to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al enviar código {Code} a {To} (isNewEmail={IsNewEmail})",
                code, to, isNewEmail);
            throw; // El cambio de correo sí debe fallar si el envío falla.
        }
    }

    // ──────────────────────────────────────────────────────────────────
    // HTML Builders
    // ──────────────────────────────────────────────────────────────────

    private static string BuildInvitationHtml(string name, string acceptUrl)
    {
        var safeName = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(name) ? "equipo" : name);
        var safeUrl = WebUtility.HtmlEncode(acceptUrl);

        return $"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>Invitación al panel administrativo de Maro's Pijamas</title>
        </head>
        <body style="margin:0;padding:0;background-color:#f5f0ea;font-family:Arial,Helvetica,sans-serif">
          <div style="display:none;max-height:0;overflow:hidden;color:transparent">
            Has recibido una invitación para activar tu cuenta del panel administrativo de Maro's Pijamas.
          </div>
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
            <tr>
              <td align="center" style="padding:32px 16px">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;background:#ffffff;border-radius:12px;overflow:hidden">
                  <tr>
                    <td style="background:#4a5833;color:#ffffff;padding:28px 32px">
                      <p style="margin:0;font-size:22px;font-weight:bold">{ProjectName}</p>
                      <p style="margin:6px 0 0;font-size:14px;opacity:0.92">Panel administrativo</p>
                    </td>
                  </tr>
                  <tr>
                    <td style="padding:32px">
                      <p style="margin:0 0 16px;font-size:16px;color:#1c1917">Hola, <strong>{safeName}</strong></p>
                      <p style="margin:0 0 24px;font-size:14px;line-height:1.6;color:#44403c">
                        El equipo de <strong>{ProjectName}</strong> creó una cuenta para ti en el panel administrativo.
                        Para activar el acceso, confirma la invitación y define tu contraseña desde el siguiente botón.
                      </p>
                      <p style="margin:0 0 24px;font-size:14px;line-height:1.6;color:#44403c">
                        Si no esperabas esta invitación, puedes ignorar este mensaje de forma segura.
                      </p>
                      <p style="margin:0 0 24px;text-align:center">
                        <a href="{safeUrl}" style="display:inline-block;background:#4a5833;color:#ffffff;text-decoration:none;padding:12px 28px;border-radius:8px;font-size:14px;font-weight:bold">Aceptar invitación</a>
                      </p>
                      <p style="margin:0;font-size:13px;line-height:1.6;color:#78716c">
                        Si el botón no funciona, copia y pega este enlace en tu navegador:<br />
                        <a href="{safeUrl}" style="color:#4a5833;word-break:break-all">{safeUrl}</a>
                      </p>
                    </td>
                  </tr>
                  <tr>
                    <td style="background:#f5f0ea;padding:16px 32px">
                      <p style="margin:0;font-size:12px;color:#78716c">
                        © {DateTime.UtcNow.Year} {ProjectName}. Este correo fue enviado por una acción administrativa del panel.
                      </p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
    }

    private static string BuildCodeHtml(string name, string code, bool isNewEmail)
    {
        var titleText = isNewEmail ? "Confirmación de Nuevo Correo" : "Autorización de Cambio de Correo";
        var descriptionText = isNewEmail
            ? "Se ha solicitado asociar esta dirección de correo a tu cuenta en <strong>Maro's Pijamas</strong>. Para confirmar que tienes acceso a este correo, ingresa el siguiente código de verificación:"
            : "Se ha iniciado una solicitud para cambiar la dirección de correo asociada a tu cuenta en <strong>Maro's Pijamas</strong>. Tu código de autorización de seguridad es:";

        return $"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>{titleText}</title>
        </head>
        <body style="margin:0;padding:0;background-color:#f5f0ea;font-family:Arial,Helvetica,sans-serif">
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
            <tr>
              <td align="center" style="padding:32px 16px">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;background:#ffffff;border-radius:12px;overflow:hidden">
                  <tr>
                    <td style="background:#4a5833;color:#ffffff;padding:28px 32px">
                      <p style="margin:0;font-size:22px;font-weight:bold">Maro's Pijamas</p>
                      <p style="margin:4px 0 0;font-size:14px;opacity:0.9">{titleText}</p>
                    </td>
                  </tr>
                  <tr>
                    <td style="padding:32px">
                      <p style="margin:0 0 16px;font-size:16px;color:#1c1917">Hola, <strong>{name}</strong></p>
                      <p style="margin:0 0 24px;font-size:14px;line-height:1.6;color:#44403c">
                        {descriptionText}
                      </p>
                      <div style="margin:24px 0;text-align:center">
                        <span style="display:inline-block;background:#f5f0ea;border:2px dashed #4a5833;color:#1c1917;letter-spacing:6px;padding:14px 28px;border-radius:8px;font-size:28px;font-weight:bold;font-family:monospace">
                          {code}
                        </span>
                      </div>
                      <p style="margin:0;font-size:13px;line-height:1.6;color:#78716c">
                        Este código expirará en <strong>15 minutos</strong>. Si tú no solicitaste este cambio, puedes ignorar este correo de forma segura.
                      </p>
                    </td>
                  </tr>
                  <tr>
                    <td style="background:#f5f0ea;padding:16px 32px">
                      <p style="margin:0;font-size:12px;color:#78716c">
                        © {DateTime.UtcNow.Year} Maro's Pijamas. Por favor no respondas a este correo.
                      </p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
    }
}
