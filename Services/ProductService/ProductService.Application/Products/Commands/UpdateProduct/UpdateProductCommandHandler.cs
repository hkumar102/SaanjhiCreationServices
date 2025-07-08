using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Persistence;
using ProductService.Domain.Entities;

namespace ProductService.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(ProductDbContext db, IMapper mapper) : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .Include(p => p.Media)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found");

        

        db.ProductMedia.RemoveRange(product.Media);
        await db.SaveChangesAsync(cancellationToken);
        mapper.Map(request, product);
        product.Media = mapper.Map<List<ProductMedia>>(request.Media);
        await db.SaveChangesAsync(cancellationToken);
    }
}
