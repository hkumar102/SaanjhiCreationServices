using ProductService.Application.Categories.Commands.UpdateCategory;
using FluentValidation;

namespace ProductService.Application.Categories.Validators;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required");
            
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters");
            
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Category description cannot exceed 500 characters");
    }
}
