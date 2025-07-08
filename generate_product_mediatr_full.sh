#!/bin/bash

BASE_DIR="Services/ProductService/ProductService.Application/Products"
MAPPING_DIR="Services/ProductService/ProductService.Application/Mappings"

mkdir -p "$BASE_DIR/Commands/CreateProduct"
cat > "$BASE_DIR/Commands/CreateProduct/CreateProductCommand.cs" <<EOF
using MediatR;
using Shared.Contracts.Products;

namespace ProductService.Application.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Guid>
{
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
    public List<ProductMediaDto> Media { get; set; } = new();
}
EOF

cat > "$BASE_DIR/Commands/CreateProduct/CreateProductCommandHandler.cs" <<EOF
using AutoMapper;
using MediatR;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly ProductServiceDbContext _db;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(ProductServiceDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = _mapper.Map<Product>(request);
        product.Id = Guid.NewGuid();

        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
EOF

mkdir -p "$BASE_DIR/Commands/UpdateProduct"

cat > "$BASE_DIR/Commands/UpdateProduct/UpdateProductCommand.cs" <<EOF
using MediatR;
using Shared.Contracts.Products;

namespace ProductService.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest
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

cat > "$BASE_DIR/Commands/UpdateProduct/UpdateProductCommandHandler.cs" <<EOF
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Persistence;
using ProductService.Domain.Entities;

namespace ProductService.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly ProductServiceDbContext _db;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(ProductServiceDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .Include(p => p.Media)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found");

        _mapper.Map(request, product);

        _db.ProductMedia.RemoveRange(product.Media);
        product.Media = _mapper.Map<List<ProductMedia>>(request.Media);

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
EOF

mkdir -p "$BASE_DIR/Commands/DeleteProduct"

cat > "$BASE_DIR/Commands/DeleteProduct/DeleteProductCommand.cs" <<EOF
using MediatR;

namespace ProductService.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest
{
    public Guid Id { get; set; }
}
EOF

cat > "$BASE_DIR/Commands/DeleteProduct/DeleteProductCommandHandler.cs" <<EOF
using MediatR;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly ProductServiceDbContext _db;

    public DeleteProductCommandHandler(ProductServiceDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found");

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
EOF

mkdir -p "$BASE_DIR/Queries/GetProductById"

cat > "$BASE_DIR/Queries/GetProductById/GetProductByIdQuery.cs" <<EOF
using MediatR;
using Shared.Contracts.Products;

namespace ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public Guid Id { get; set; }
}
EOF

cat > "$BASE_DIR/Queries/GetProductById/GetProductByIdQueryHandler.cs" <<EOF
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Persistence;
using Shared.Contracts.Products;

namespace ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly ProductServiceDbContext _db;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(ProductServiceDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Media)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found");

        return _mapper.Map<ProductDto>(product);
    }
}
EOF

mkdir -p "$MAPPING_DIR"

cat > "$MAPPING_DIR/ProductMappingProfile.cs" <<EOF
using AutoMapper;
using ProductService.Application.Products.Commands.CreateProduct;
using ProductService.Application.Products.Commands.UpdateProduct;
using ProductService.Domain.Entities;
using Shared.Contracts.Products;

namespace ProductService.Application.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media));

        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media));

        CreateMap<ProductMediaDto, ProductMedia>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => (MediaType)src.MediaType));

        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name))
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media));

        CreateMap<ProductMedia, ProductMediaDto>()
            .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => (int)src.MediaType));
    }
}
EOF

mkdir -p "$BASE_DIR/Queries/GetAllProducts"

cat > "$BASE_DIR/Queries/GetAllProducts/GetAllProductsQuery.cs" <<EOF
using MediatR;
using Shared.Contracts.Common;
using Shared.Contracts.Products;

namespace ProductService.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<PaginatedResult<ProductDto>>
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsRentable { get; set; }
    public bool? IsActive { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public decimal? MinRentalPrice { get; set; }
    public decimal? MaxRentalPrice { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
EOF

cat > "$BASE_DIR/Queries/GetAllProducts/GetAllProductsQueryHandler.cs" <<EOF
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Persistence;
using Shared.Contracts.Common;
using Shared.Contracts.Products;

namespace ProductService.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedResult<ProductDto>>
{
    private readonly ProductServiceDbContext _db;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(ProductServiceDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Media)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(p =>
                p.Name.Contains(request.Search) ||
                (p.Description != null && p.Description.Contains(request.Search)));
        }

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.IsRentable.HasValue)
            query = query.Where(p => p.IsRentable == request.IsRentable);

        if (request.IsActive.HasValue)
            query = query.Where(p => p.IsActive == request.IsActive);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        if (request.MinRentalPrice.HasValue)
            query = query.Where(p => p.RentalPrice >= request.MinRentalPrice.Value);

        if (request.MaxRentalPrice.HasValue)
            query = query.Where(p => p.RentalPrice <= request.MaxRentalPrice.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ProductDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
EOF
