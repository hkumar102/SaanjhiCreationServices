using System;

namespace Shared.Contracts.Products;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
