using RentalService.Contracts.Enums;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RentalService.Application.Rentals.Commands.Create;
using RentalService.Domain.Entities;
using RentalService.Infrastructure.Persistence;

public class CreateRentalCommandHandler(RentalDbContext dbContext, IMapper mapper, ILogger<CreateRentalCommandHandler> logger)
    : IRequestHandler<CreateRentalCommand, Guid>
{
    public async Task<Guid> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("CreateRentalCommand received for ProductId: {ProductId}, InventoryItemId: {InventoryItemId}, CustomerId: {CustomerId}", request.ProductId, request.InventoryItemId, request.CustomerId);
        var entity = mapper.Map<Rental>(request);
        entity.Status = RentalStatus.Pending;

        // Generate user-friendly RentalNumber: RNT-YYYYMMDD-XXXXX
        var today = DateTime.UtcNow.Date;
        var countToday = await dbContext.Rentals.CountAsync(r => r.CreatedAt.Date == today, cancellationToken);
        entity.RentalNumber = $"RNT-{today:yyyyMMdd}-{countToday + 1:D5}";

        logger.LogDebug("Rental entity mapped, status set to Pending, and RentalNumber generated: {RentalNumber}", entity.RentalNumber);
        dbContext.Rentals.Add(entity);

        dbContext.RentalTimelines.Add(new RentalTimeline
        {
            RentalId = entity.Id,
            Status = (int)entity.Status,
            Notes = request.Notes,
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogDebug("DbContext changes saved for new RentalId: {RentalId}", entity.Id);
        return entity.Id;
    }
}