using FluentValidation;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Validators;

public class ProductMediaDtoValidator : AbstractValidator<ProductMediaDto>
{
    public ProductMediaDtoValidator()
    {
        RuleFor(x => x.Url).NotEmpty();
        RuleFor(x => x.PublicId).NotEmpty();
    }
}
