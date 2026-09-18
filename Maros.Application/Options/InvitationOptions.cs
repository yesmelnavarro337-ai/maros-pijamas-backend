namespace Maros.Application.Options;

public class InvitationOptions
{
    public const string SectionName = "Invitation";

    /// <summary>
    /// URL pública raíz del panel administrativo.
    /// </summary>
    public string FrontendUrl { get; set; } = "https://maros-admin.vercel.app";

    /// <summary>
    /// URL completa legacy del enlace de aceptación. Se conserva como fallback
    /// para despliegues que aún no definan FrontendUrl.
    /// </summary>
    public string? AcceptUrl { get; set; }

    /// <summary>Horas de validez del enlace de invitación.</summary>
    public int ExpiresInHours { get; set; } = 48;
}
