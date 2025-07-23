using RentalService.Application.Rentals.Commands.Create;
using FluentValidation;

namespace RentalService.Application.Rentals.Validators;


public class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ShippingAddressId).NotEmpty();
        RuleFor(x => x.InventoryItemId).NotEmpty();
        RuleFor(x => x.BookNumber).GreaterThan(0);
        RuleFor(x => x.StartDate).LessThanOrEqualTo(x => x.EndDate);
        RuleFor(x => x.RentalPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SecurityDeposit).GreaterThanOrEqualTo(0);
    }
}