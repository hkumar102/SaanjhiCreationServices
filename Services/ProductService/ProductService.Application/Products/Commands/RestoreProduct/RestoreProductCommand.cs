using MediatR;

namespace ProductService.Application.Products.Commands.RestoreProduct;

public record RestoreProductCommand(Guid Id) : IRequest;
