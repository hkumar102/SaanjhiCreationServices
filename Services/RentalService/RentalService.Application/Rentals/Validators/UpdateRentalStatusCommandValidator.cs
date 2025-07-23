using FluentValidation;
using RentalService.Application.Rentals.Commands.UpdateStatus;
using RentalService.Contracts.Enums;

namespace RentalService.Application.Rentals.Validators;

public class UpdateRentalStatusCommandValidator : AbstractValidator<UpdateRentalStatusCommand>
{
    public UpdateRentalStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        // ActualStartDate required for PickedUp or Booked
        When(x => x.Status == RentalStatus.PickedUp, () =>
        {
            RuleFor(x => x.ActualStartDate).NotNull().WithMessage("ActualStartDate is required when marking as PickedUp / Delivered.");
        });
        // ActualReturnDate required for Returned
        When(x => x.Status == RentalStatus.Returned, () =>
        {
            RuleFor(x => x.ActualReturnDate).NotNull().WithMessage("ActualReturnDate is required when marking as Returned.");
        });
    }
}
