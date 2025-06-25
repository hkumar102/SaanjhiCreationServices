using System;

namespace Shared.Contracts.Products;

public class ProductMediaDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public int MediaType { get; set; }
}
