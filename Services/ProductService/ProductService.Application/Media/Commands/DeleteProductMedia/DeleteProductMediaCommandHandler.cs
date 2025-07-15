
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Media.Commands.DeleteProductMedia;

public class DeleteProductMediaCommandHandler(
    ProductDbContext db,
    ILogger<DeleteProductMediaCommandHandler> logger)
    : IRequestHandler<DeleteProductMediaCommand>
{
    public async Task Handle(DeleteProductMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await db.ProductMedia.FirstOrDefaultAsync(m => m.Id == request.MediaId, cancellationToken);
        if (media == null)
        {
            throw new KeyNotFoundException($"Media with ID {request.MediaId} not found");
        }

        db.ProductMedia.Remove(media);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Deleted product media with ID: {MediaId}", request.MediaId);
    }
}