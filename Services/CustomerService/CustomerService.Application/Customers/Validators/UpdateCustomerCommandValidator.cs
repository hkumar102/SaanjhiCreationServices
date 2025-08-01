using FluentValidation;
using CustomerService.Application.Customers.Commands.Update;

namespace CustomerService.Application.Customers.Validators;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
    }
}