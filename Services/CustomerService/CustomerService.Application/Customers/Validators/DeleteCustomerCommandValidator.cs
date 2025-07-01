using FluentValidation;
using CustomerService.Application.Customers.Commands.Delete;

namespace CustomerService.Application.Customers.Validators;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}