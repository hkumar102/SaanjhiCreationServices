using FluentValidation;

namespace ProductService.Application.Media.Commands.UploadProductMedia;

public class UploadProductMediaCommandValidator : AbstractValidator<UploadProductMediaCommand>
{
    public UploadProductMediaCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");

        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.");

        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("Color must be at most 50 characters.");

        RuleFor(x => x.AltText)
            .MaximumLength(500).WithMessage("AltText must be at most 500 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be non-negative.");
    }
}
