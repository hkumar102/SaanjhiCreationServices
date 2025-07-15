using FluentValidation;
using ProductService.Application.Products.Commands.AddProductMedia;

namespace ProductService.Application.Products.Validators;

public class AddProductMediaCommandValidator : AbstractValidator<AddProductMediaCommand>
{
    private readonly string[] AllowedMediaTypes = { "image", "video", "document" };

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
            .NotEmpty()
            .WithMessage("Media type is required")
            .Must(x => AllowedMediaTypes.Contains(x.ToLower()))
            .WithMessage($"Media type must be one of: {string.Join(", ", AllowedMediaTypes)}");

        RuleFor(x => x.Color)
            .MaximumLength(50)
            .WithMessage("Color cannot exceed 50 characters")
            .When(x => !x.IsGeneric);

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Sort order must be non-negative")
            .LessThan(1000)
            .WithMessage("Sort order cannot exceed 999");

        RuleFor(x => x.AltText)
            .MaximumLength(200)
            .WithMessage("Alt text cannot exceed 200 characters");

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
