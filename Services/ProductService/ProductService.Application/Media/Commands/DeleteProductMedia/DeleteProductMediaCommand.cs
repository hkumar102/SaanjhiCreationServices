using MediatR;

namespace ProductService.Application.Media.Commands.DeleteProductMedia;

public record DeleteProductMediaCommand(Guid MediaId) : IRequest;