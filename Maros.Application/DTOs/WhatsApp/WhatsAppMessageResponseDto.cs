namespace Maros.Application.DTOs.WhatsApp;

public record WhatsAppMessageResponseDto(
    string PhoneNumber,
    string Message,
    string Link
);