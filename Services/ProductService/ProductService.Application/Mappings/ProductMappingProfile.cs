using AutoMapper;
using ProductService.Application.Products.Commands.CreateProduct;
using ProductService.Application.Products.Commands.UpdateProduct;
using ProductService.Application.Categories.Commands.CreateCategory;
using ProductService.Application.Categories.Commands.UpdateCategory;
using ProductService.Contracts.DTOs;
using ProductService.Domain.Entities;
using System.Text.Json;

namespace ProductService.Application.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Command mappings
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore());

        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            .ForMember(dest => dest.SKU, opt => opt.Ignore());

        // Category command mappings
        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore());

        CreateMap<UpdateCategoryCommand, Category>()
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore());

        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            .ForMember(dest => dest.InventoryItems, opt => opt.MapFrom(src => src.InventoryItems))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.TotalInventoryCount, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableInventoryCount, opt => opt.Ignore())
            .ForMember(dest => dest.MediaByColor, opt => opt.Ignore())
            .ForMember(dest => dest.GenericMedia, opt => opt.Ignore());

        // Category mappings
        CreateMap<Category, CategoryDto>();

        // Inventory mappings
        CreateMap<InventoryItem, InventoryItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductBrand, opt => opt.MapFrom(src => src.Product.Brand))
            .ForMember(dest => dest.AvailableSizes, opt => opt.MapFrom(src => src.Product.AvailableSizes))
            .ForMember(dest => dest.AvailableColors, opt => opt.MapFrom(src => src.Product.AvailableColors))
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Product.Media));

        // Media mappings
        CreateMap<ProductMediaDto, ProductMedia>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.VariantsJson, opt => opt.MapFrom(src =>
                SerializeVariants(src.Variants)))
            .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => (Shared.Contracts.Media.MediaType)src.MediaType));

        CreateMap<ProductMedia, ProductMediaDto>()
            .ForMember(dest => dest.Variants, opt => opt.MapFrom(src =>
                DeserializeVariants(src.VariantsJson)))
            .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => (int)src.MediaType));
    }

    private static string? SerializeVariants(MediaVariantsDto? variants)
    {
        return variants != null ? JsonSerializer.Serialize(variants) : null;
    }

    private static MediaVariantsDto? DeserializeVariants(string? variantsJson)
    {
        return !string.IsNullOrEmpty(variantsJson)
            ? JsonSerializer.Deserialize<MediaVariantsDto>(variantsJson)
            : null;
    }
}
