#!/bin/bash

SERVICE_NAME="ProductService"
DOMAIN_DIR="./Services/$SERVICE_NAME/$SERVICE_NAME.Domain"
CONTRACTS_DIR="./Shared/Contracts/Products"

echo "Creating directories..."
mkdir -p "$DOMAIN_DIR/Entities"
mkdir -p "$DOMAIN_DIR/Configurations"
mkdir -p "$CONTRACTS_DIR"

echo "Generating Product entity..."
cat > "$DOMAIN_DIR/Entities/Product.cs" <<EOF
using Shared.Domain;

namespace $SERVICE_NAME.Domain.Entities;

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
EOF

echo "Generating ProductMedia entity..."
cat > "$DOMAIN_DIR/Entities/ProductMedia.cs" <<EOF
using Shared.Domain;

namespace $SERVICE_NAME.Domain.Entities;

public class ProductMedia : BaseEntity
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public MediaType MediaType { get; set; }

    public Product Product { get; set; } = null!;
}
EOF

echo "Generating MediaType enum..."
cat > "$DOMAIN_DIR/Entities/MediaType.cs" <<EOF
namespace $SERVICE_NAME.Domain.Entities;

public enum MediaType
{
    Image = 1,
    Video = 2
}
EOF

echo "Generating Category entity..."
cat > "$DOMAIN_DIR/Entities/Category.cs" <<EOF
using Shared.Domain;

namespace $SERVICE_NAME.Domain.Entities;

public class Category : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
EOF

echo "Generating DTOs..."
cat > "$CONTRACTS_DIR/ProductDto.cs" <<EOF
namespace Shared.Contracts.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsActive { get; set; }
    public bool IsRentable { get; set; }
    public decimal? RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int? MaxRentalDays { get; set; }
    public Guid CategoryId { get; set; }
    public List<ProductMediaDto> Media { get; set; } = new();
}
EOF

cat > "$CONTRACTS_DIR/ProductMediaDto.cs" <<EOF
namespace Shared.Contracts.Products;

public class ProductMediaDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public int MediaType { get; set; }
}
EOF

cat > "$CONTRACTS_DIR/CategoryDto.cs" <<EOF
namespace Shared.Contracts.Products;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
EOF


echo "✅ Product, Category, and ProductMedia structure generated."
