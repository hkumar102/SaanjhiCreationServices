using FluentValidation;
using ProductService.Application.Products.Commands.AddInventoryItem;

namespace ProductService.Application.Products.Validators;

public class AddInventoryItemCommandValidator : AbstractValidator<AddInventoryItemCommand>
{
    public AddInventoryItemCommandValidator()
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

        RuleFor(x => x.SerialNumber)
            .MaximumLength(50)
            .WithMessage("Serial number cannot exceed 50 characters");

        RuleFor(x => x.Barcode)
            .MaximumLength(50)
            .WithMessage("Barcode cannot exceed 50 characters");

        RuleFor(x => x.ConditionNotes)
            .MaximumLength(500)
            .WithMessage("Condition notes cannot exceed 500 characters");

        RuleFor(x => x.WarehouseLocation)
            .MaximumLength(100)
            .WithMessage("Warehouse location cannot exceed 100 characters");
    }
}
