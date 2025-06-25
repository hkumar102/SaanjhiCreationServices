using Shared.Domain;
using Shared.Domain.Entities;

namespace ProductService.Domain.Entities;

public class Product : AuditableEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsRentable { get; set; } = false;

    public decimal? RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int? MaxRentalDays { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<ProductMedia> Media { get; set; } = new List<ProductMedia>();
}
