using Shared.Domain.Entities;

namespace ProductService.Domain.Entities;

/// <summary>
/// Category entity for product categorization
/// </summary>
public class Category : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    
    // Navigation Properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
