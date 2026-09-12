using System.Net;
using System.Net.Mail;
using Maros.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Maros.Infrastructure.ExternalServices;

public class SmtpEmailService : IEmailService
{
    private const int DefaultPort = 587;
    private const string DefaultFrom = "marospijamas@gmail.com";
    private const string DefaultFromName = "Maro's Pijamas";

    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _settings = configuration.GetSection("Smtp").Get<SmtpSettings>() ?? new SmtpSettings();
        _logger = logger;
    }

    public Task SendInvitationAsync(string to, string name, string acceptUrl)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            // SMTP sin configurar: la invitación ya quedó persistida (Pendiente),
            // así que se registra el enlace en el log para poder probar el flujo.
            _logger.LogWarning(
                "SMTP no configurado (Smtp:Host vacío). No se envió correo a {To}. Enlace de aceptación: {AcceptUrl}",
                to,
                acceptUrl
            );
            return Task.CompletedTask;
        }

        var message = new MailMessage
        {
            From = new MailAddress(Resolve(_settings.From, DefaultFrom), Resolve(_settings.FromName, DefaultFromName)),
            Subject = "Invitación a Maro's Pijamas",
            Body = BuildHtml(name, acceptUrl),
            IsBodyHtml = true,
        };
        message.To.Add(to);

#pragma warning disable SYSLIB0014 // SmtpClient está obsoleto, pero no se requiere paquete adicional.
        var client = new SmtpClient(_settings.Host, _settings.Port <= 0 ? DefaultPort : _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            UseDefaultCredentials = false,
        };
#pragma warning restore SYSLIB0014

        if (_settings.CredentialsRequired && !string.IsNullOrWhiteSpace(_settings.Username))
            client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);

        _logger.LogInformation("Enviando correo de invitación a {To}", to);

#pragma warning disable SYSLIB0014
        return client.SendMailAsync(message);
#pragma warning restore SYSLIB0014
    }

    private static string Resolve(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;

    private static string BuildHtml(string name, string acceptUrl) => $"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>Invitación a Maro's Pijamas</title>
        </head>
        <body style="margin:0;padding:0;background-color:#f5f0ea;font-family:Arial,Helvetica,sans-serif">
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
            <tr>
              <td align="center" style="padding:32px 16px">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;background:#ffffff;border-radius:12px;overflow:hidden">
                  <tr>
                    <td style="background:#a38a3e;color:#ffffff;padding:28px 32px">
                      <p style="margin:0;font-size:22px;font-weight:bold">Maro's Pijamas</p>
                    </td>
                  </tr>
                  <tr>
                    <td style="padding:32px">
                      <p style="margin:0 0 16px;font-size:16px;color:#1c1917">Hola, <strong>{name}</strong></p>
                      <p style="margin:0 0 24px;font-size:14px;line-height:1.6;color:#44403c">
                        Fuiste invitada(o) a unirte al panel administrativo de <strong>Maro's Pijamas</strong>.
                        Para completar tu registro, crea tu contraseña con el siguiente enlace.
                      </p>
                      <p style="margin:0 0 24px;text-align:center">
                        <a href="{acceptUrl}" style="display:inline-block;background:#a38a3e;color:#ffffff;text-decoration:none;padding:12px 28px;border-radius:8px;font-size:14px;font-weight:bold">Aceptar invitación</a>
                      </p>
                      <p style="margin:0;font-size:13px;line-height:1.6;color:#78716c">
                        Si el botón no funciona, copia y pega este enlace en tu navegador:<br />
                        <a href="{acceptUrl}" style="color:#a38a3e;word-break:break-all">{acceptUrl}</a>
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

    private class SmtpSettings
    {
        public string? Host { get; set; }
        public int Port { get; set; } = DefaultPort;
        public bool EnableSsl { get; set; } = true;
        public bool CredentialsRequired { get; set; } = true;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? From { get; set; }
        public string? FromName { get; set; }
    }
}