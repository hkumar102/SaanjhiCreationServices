using UserService.Application.Commands;

namespace UserService.Application.Validators;

using FluentValidation;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        // RuleFor(x => x.PhotoUrl).Must(BeValidUrl).When(x => x.PhotoUrl != null);
    }
}