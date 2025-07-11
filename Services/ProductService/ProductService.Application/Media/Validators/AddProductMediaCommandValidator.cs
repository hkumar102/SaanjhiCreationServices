using FluentValidation;
using ProductService.Application.Media.Commands.AddProductMedia;

namespace ProductService.Application.Media.Validators;

public class AddProductMediaCommandValidator : AbstractValidator<AddProductMediaCommand>
{
    private readonly string[] AllowedPurposes = { "main", "detail", "style", "size-guide", "care-instructions", "video" };

    public AddProductMediaCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("URL is required")
            .Must(BeValidUrl)
            .WithMessage("URL must be a valid HTTP/HTTPS URL")
            .MaximumLength(1000)
            .WithMessage("URL cannot exceed 1000 characters");

        RuleFor(x => x.MediaType)
            .IsInEnum()
            .WithMessage("Invalid media type value");

        RuleFor(x => x.Color)
            .MaximumLength(50)
            .WithMessage("Color cannot exceed 50 characters")
            .When(x => !x.IsGeneric);

        RuleFor(x => x.Purpose)
            .MaximumLength(50)
            .WithMessage("Purpose cannot exceed 50 characters")
            .Must(x => string.IsNullOrEmpty(x) || AllowedPurposes.Any(purpose => string.Equals(purpose, x, StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"Purpose must be one of: {string.Join(", ", AllowedPurposes)}")
            .When(x => !string.IsNullOrEmpty(x.Purpose));

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Sort order must be non-negative")
            .LessThan(1000)
            .WithMessage("Sort order cannot exceed 999");

        RuleFor(x => x.AltText)
            .MaximumLength(200)
            .WithMessage("Alt text cannot exceed 200 characters");

        RuleFor(x => x.Caption)
            .MaximumLength(500)
            .WithMessage("Caption cannot exceed 500 characters");

        // Business rules
        RuleFor(x => x)
            .Must(x => x.IsGeneric || !string.IsNullOrEmpty(x.Color))
            .WithMessage("Color is required when media is not generic");
    }

    private bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
