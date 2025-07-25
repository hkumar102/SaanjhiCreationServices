using FluentValidation;
using ProductService.Application.Products.Commands.UpdateProduct;

namespace ProductService.Application.Products.Validators;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        // ID validation
        RuleFor(x => x.Id).NotEmpty();

        // Basic Information
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Brand).MaximumLength(50);
        RuleFor(x => x.Designer).MaximumLength(50);
        RuleFor(x => x.SKU).MaximumLength(50);

        // Pricing
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RentalPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SecurityDeposit).GreaterThanOrEqualTo(0).When(x => x.SecurityDeposit.HasValue);
        RuleFor(x => x.MaxRentalDays).GreaterThan(0).LessThanOrEqualTo(365);

        // Product Specifications
        RuleFor(x => x.Material).MaximumLength(100);
        RuleFor(x => x.CareInstructions).MaximumLength(500);
        RuleFor(x => x.Season).MaximumLength(50);

        // Relationships
        RuleFor(x => x.CategoryId).NotEmpty();
        
        RuleForEach(x => x.Media).SetValidator(new ProductMediaDtoValidator());
    }
}
