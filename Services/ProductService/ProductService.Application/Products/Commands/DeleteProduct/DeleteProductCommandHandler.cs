using MediatR;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(ProductDbContext db) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await db.Products.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found");

        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);
    }
}
