namespace Maros.Application.Common;

public record ApiErrorResponse(
    int Status,
    string Message,
    Dictionary<string, string[]>? Errors = null
);