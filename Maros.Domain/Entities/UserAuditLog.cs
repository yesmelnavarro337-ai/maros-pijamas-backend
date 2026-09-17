using Maros.Domain.Common;

namespace Maros.Domain.Entities;

public class UserAuditLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;

    public User? User { get; set; }
}
