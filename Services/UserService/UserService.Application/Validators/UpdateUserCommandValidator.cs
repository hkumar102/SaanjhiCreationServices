using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators;

/// <summary>
/// Validator for UpdateUserCommand.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
