using MediatR;

namespace ProductService.Application.Categories.Commands.RestoreCategory;

public record RestoreCategoryCommand(Guid Id) : IRequest;
