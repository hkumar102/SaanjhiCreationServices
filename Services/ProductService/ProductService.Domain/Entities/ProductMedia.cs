using Shared.Contracts.Media;
using Shared.Domain.Entities;

namespace ProductService.Domain.Entities;

public class ProductMedia : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public MediaType MediaType { get; set; }
    public Product Product { get; set; } = null!;
}
