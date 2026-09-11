namespace Maros.Application.Interfaces;

public interface IEmailService
{
    Task SendInvitationAsync(string to, string name, string acceptUrl);
}