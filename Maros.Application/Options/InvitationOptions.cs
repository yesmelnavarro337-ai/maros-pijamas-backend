namespace Maros.Application.Options;

public class InvitationOptions
{
    public const string SectionName = "Invitation";

    /// <summary>
    /// URL pública (frontend) a la que apunta el enlace de aceptación,
    /// a la que se le añade "?token={token}".
    /// </summary>
    public string AcceptUrl { get; set; } = "http://localhost:3000/accept-invite";

    /// <summary>Horas de validez del enlace de invitación.</summary>
    public int ExpiresInHours { get; set; } = 48;
}