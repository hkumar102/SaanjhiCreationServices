using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators;

/// <summary>
/// Validator for AddShippingAddressCommand.
/// </summary>
public class AddShippingAddressCommandValidator : AbstractValidator<AddShippingAddressCommand>
{
    public AddShippingAddressCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AddressLine1).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
        RuleFor(x => x.PostalCode).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}
