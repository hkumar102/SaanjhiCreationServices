using FluentValidation;
using CustomerService.Application.Addresses.Commands.Delete;

namespace CustomerService.Application.Addresses.Validators;

public class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
{
    public DeleteAddressCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}