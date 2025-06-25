using MediatR;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly ProductDbContext _db;

    public DeleteProductCommandHandler(ProductDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found");

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
