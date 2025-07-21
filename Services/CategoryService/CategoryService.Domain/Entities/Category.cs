using Shared.Domain.Entities;

namespace CategoryService.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
