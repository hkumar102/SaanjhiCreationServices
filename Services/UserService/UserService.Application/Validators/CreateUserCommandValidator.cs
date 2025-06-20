using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators;

/// <summary>
/// Validator for CreateUserCommand.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirebaseUserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
