using FluentValidation;
using Shared.Contracts.Media;
using Shared.Contracts.Products;

namespace ProductService.Application.Products.Validators;

public class ProductMediaDtoValidator : AbstractValidator<ProductMediaDto>
{
    public ProductMediaDtoValidator()
    {
        RuleFor(x => x.Url).NotEmpty();
        RuleFor(x => x.PublicId).NotEmpty();
    }
}
