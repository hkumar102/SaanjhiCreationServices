using UserService.Application.Commands;

namespace UserService.Application.Validators;

using FluentValidation;

public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleForEach(x => x.Roles).NotEmpty();
    }
}