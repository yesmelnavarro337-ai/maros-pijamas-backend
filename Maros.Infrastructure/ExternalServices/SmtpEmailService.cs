using MailKit.Net.Smtp;
using MailKit.Security;
using Maros.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net;

namespace Maros.Infrastructure.ExternalServices;

/// <summary>
/// Implementación de <see cref="IEmailService"/> usando MailKit + Gmail SMTP.
/// </summary>
public sealed class SmtpEmailService : IEmailService
{
    private const int DefaultPort = 587;
    private const string DefaultFrom = "marospijamas@gmail.com";
    private const string ProjectName = "Maro's Pijamas";
    private const string DefaultFromName = "Maro's Pijamas - Panel Administrativo";
    private const string GmailSmtpServer = "smtp.gmail.com";

    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task SendInvitationAsync(string to, string name, string acceptUrl)
    {
        var server = Resolve(_configuration["Smtp:Server"], _configuration["Smtp:Host"] ?? string.Empty);
        if (string.IsNullOrWhiteSpace(server))
        {
            _logger.LogWarning(
                "SMTP no configurado (Smtp:Server vacío). No se envió correo a {To}. Enlace de aceptación: {AcceptUrl}",
                to, acceptUrl);
            return;
        }

        var message = BuildMessage(
            to,
            subject: "Activa tu cuenta del panel administrativo de Maro's Pijamas",
            bodyHtml: BuildInvitationHtml(name, acceptUrl),
            bodyText: BuildInvitationText(name, acceptUrl));

        _logger.LogInformation("Enviando correo de invitación a {To}", to);
        await SendAsync(message);
        _logger.LogInformation("Correo de invitación enviado a {To}", to);
    }

    /// <inheritdoc/>
    public async Task SendEmailChangeCodeAsync(string to, string name, string code, bool isNewEmail)
    {
        var server = Resolve(_configuration["Smtp:Server"], _configuration["Smtp:Host"] ?? string.Empty);
        if (string.IsNullOrWhiteSpace(server))
        {
            _logger.LogWarning(
                "SMTP no configurado (Smtp:Server vacío). Código {Code} no enviado a {To} (isNewEmail={IsNewEmail})",
                code, to, isNewEmail);
            return;
        }

        var subject = isNewEmail
            ? "Código de verificación de nuevo correo — Maro's Pijamas"
            : "Código de seguridad para cambio de correo — Maro's Pijamas";

        var message = BuildMessage(to, subject, BuildCodeHtml(name, code, isNewEmail));

        _logger.LogInformation("Enviando código de verificación a {To} (isNewEmail={IsNewEmail})", to, isNewEmail);
        await SendAsync(message);
        _logger.LogInformation("Código {Code} enviado a {To}", code, to);
    }

    // ──────────────────────────────────────────────────────────────────
    // Helpers privados con MailKit / MimeKit
    // ──────────────────────────────────────────────────────────────────

    private MimeMessage BuildMessage(string to, string subject, string bodyHtml, string? bodyText = null)
    {
        var fromEmail = Resolve(_configuration["Smtp:SenderEmail"], Resolve(_configuration["Smtp:From"], DefaultFrom));
        var fromName = Resolve(_configuration["Smtp:SenderName"], Resolve(_configuration["Smtp:FromName"], DefaultFromName));

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.ReplyTo.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Headers.Add("X-Entity-Ref-ID", Guid.NewGuid().ToString("N"));

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = bodyHtml,
            TextBody = bodyText
        };
        message.Body = bodyBuilder.ToMessageBody();

        return message;
    }

    private async Task SendAsync(MimeMessage message)
    {
        var host = Resolve(_configuration["Smtp:Server"], Resolve(_configuration["Smtp:Host"], GmailSmtpServer));
        var portStr = _configuration["Smtp:Port"];
        var port = int.TryParse(portStr, out var p) && p > 0 ? p : DefaultPort;

        var username = Resolve(
            _configuration["Smtp:Username"],
            Resolve(_configuration["Smtp:SenderEmail"], Resolve(_configuration["Smtp:From"], DefaultFrom)));
        var rawPassword = _configuration["Smtp:Password"] ?? string.Empty;
        var password = rawPassword.Replace(" ", "").Trim();
        var timeoutStr = _configuration["Smtp:TimeoutMilliseconds"];
        var timeoutMilliseconds = int.TryParse(timeoutStr, out var configuredTimeout) && configuredTimeout > 0
            ? configuredTimeout
            : 10_000;

        using var client = new SmtpClient();
        client.Timeout = timeoutMilliseconds;

        // Conexión segura con STARTTLS para el puerto 587
        using var cts = new CancellationTokenSource(timeoutMilliseconds);
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, cts.Token);

        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            await client.AuthenticateAsync(username, password, cts.Token);
        }

        await client.SendAsync(message, cts.Token);
        await client.DisconnectAsync(true, cts.Token);
    }

    private static string Resolve(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;

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

    private static string BuildInvitationText(string name, string acceptUrl)
    {
        var displayName = string.IsNullOrWhiteSpace(name) ? "equipo" : name;

        return $"""
        Hola, {displayName}.

        El equipo de {ProjectName} creó una cuenta para ti en el panel administrativo.
        Para activar el acceso, abre este enlace y define tu contraseña:

        {acceptUrl}

        Si no esperabas esta invitación, puedes ignorar este mensaje.

        {ProjectName}
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
