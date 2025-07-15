using FluentValidation;
using ProductService.Application.Inventory.Commands.UpdateInventoryItem;

namespace ProductService.Application.Inventory.Validators;

public class UpdateInventoryItemCommandValidator : AbstractValidator<UpdateInventoryItemCommand>
{
    public UpdateInventoryItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Inventory item ID is required.");

        RuleFor(x => x.Size)
            .MaximumLength(20).When(x => x.Size != null);

        RuleFor(x => x.Color)
            .MaximumLength(50).When(x => x.Color != null);

        RuleFor(x => x.ConditionNotes)
            .MaximumLength(500).When(x => x.ConditionNotes != null);

        RuleFor(x => x.WarehouseLocation)
            .MaximumLength(100).When(x => x.WarehouseLocation != null);

        RuleFor(x => x.AcquisitionCost)
            .GreaterThanOrEqualTo(0).When(x => x.AcquisitionCost.HasValue);
    }
}
