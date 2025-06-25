using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Persistence;
using ProductService.Domain.Entities;

namespace ProductService.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly ProductDbContext _db;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(ProductDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .Include(p => p.Media)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found");

        

        _db.ProductMedia.RemoveRange(product.Media);
        await _db.SaveChangesAsync(cancellationToken);
        _mapper.Map(request, product);
        product.Media = _mapper.Map<List<ProductMedia>>(request.Media);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
