using FluentValidation;
using ProductService.Application.Inventory.Commands.CreateInventoryItem;

namespace ProductService.Application.Inventory.Validators;

public class CreateInventoryItemCommandValidator : AbstractValidator<CreateInventoryItemCommand>
{
    public CreateInventoryItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Size)
            .NotEmpty()
            .WithMessage("Size is required")
            .MaximumLength(10)
            .WithMessage("Size cannot exceed 10 characters");

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Color is required")
            .MaximumLength(50)
            .WithMessage("Color cannot exceed 50 characters");

        RuleFor(x => x.Condition)
            .IsInEnum()
            .WithMessage("Invalid condition value");

        RuleFor(x => x.AcquisitionCost)
            .GreaterThan(0)
            .WithMessage("Acquisition cost must be greater than 0")
            .LessThan(100000)
            .WithMessage("Acquisition cost cannot exceed 100,000");

        RuleFor(x => x.AcquisitionDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Acquisition date cannot be in the future")
            .When(x => x.AcquisitionDate.HasValue);

        RuleFor(x => x.ConditionNotes)
            .MaximumLength(1000)
            .WithMessage("Condition notes cannot exceed 1000 characters");
    }
}
