using Shared.Domain.Entities;

namespace CategoryService.Domain.Entities;

public class Category : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
