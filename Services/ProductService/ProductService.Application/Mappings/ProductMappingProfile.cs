using AutoMapper;
using ProductService.Application.Products.Commands.CreateProduct;
using ProductService.Application.Products.Commands.UpdateProduct;
using ProductService.Domain.Entities;
using Shared.Contracts.Media;
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
