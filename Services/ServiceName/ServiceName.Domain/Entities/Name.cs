using Shared.Domain.Entities;

namespace services.ServiceName.ServiceName.Domain.Entities;

public class Name : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
