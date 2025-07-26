using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.ErrorHandling;
using Microsoft.Extensions.Logging;
using RentalService.Contracts.Enums;
using RentalService.Infrastructure.Persistence;
using RentalService.Infrastructure.HttpClients;
using RentalService.Contracts.DTOs;
using RentalService.Application.Notifications.Commands;
using NotificationService.Contracts.Enums;
using RentalService.Application.Rentals.Queries.GetRentalById;

namespace RentalService.Application.Rentals.Commands.UpdateStatus;

public class UpdateRentalStatusCommandHandler : IRequestHandler<UpdateRentalStatusCommand>
{
    private readonly RentalDbContext dbContext;
    private readonly ILogger<UpdateRentalStatusCommandHandler> logger;
    private readonly IProductApiClient productApiClient;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly string _baseUrl;


    public UpdateRentalStatusCommandHandler(
        RentalDbContext dbContext,
        IProductApiClient productApiClient,
        ILogger<UpdateRentalStatusCommandHandler> logger,
        IMediator mediator,
        IMapper mapper,
        IConfiguration configuration)
    {
        this.dbContext = dbContext;
        this.productApiClient = productApiClient;
        this.logger = logger;
        this._mediator = mediator;
        this._mapper = mapper;
        _baseUrl = configuration["App:AdminPortalBaseUrl"] ?? "https://www.saanjhicreation.com";

    }

    public async Task Handle(UpdateRentalStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("UpdateRentalStatusCommand received for RentalId: {RentalId} to Status: {Status}", request.Id, request.Status);
        var entity = await dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (entity == null)
        {
            logger.LogDebug("Rental not found for Id: {RentalId}", request.Id);
            throw new KeyNotFoundException("Rental not found.");
        }

        if (!IsValidTransition(entity.Status, request.Status))
        {
            logger.LogDebug("Invalid status transition from {CurrentStatus} to {NextStatus} for RentalId: {RentalId}", entity.Status, request.Status, request.Id);
            throw new BusinessRuleException($"Cannot transition from {entity.Status} to {request.Status}.");
        }

        var inventoryItem = await productApiClient.GetInventoryByIdAsync(entity.InventoryItemId, cancellationToken);

        if (inventoryItem == null)
        {
            logger.LogError("Inventory item with ID {InventoryItemId} not found", entity.InventoryItemId);
            throw new BusinessRuleException($"Inventory item with ID {entity.InventoryItemId} not found");
        }

        // Block transition to Booked or PickedUp if inventory item is not available (retired, damaged, lost, etc.)
        if ((request.Status == RentalStatus.Booked || request.Status == RentalStatus.PickedUp)
            && (inventoryItem.Status != InventoryStatus.Available && inventoryItem.Status != InventoryStatus.Reserved))
        {
            logger.LogError($"Cannot transition rental {request.Id} to {request.Status.ToString()} because inventory item {entity.InventoryItemId} is not available (current status: {inventoryItem.Status.ToString()})");
            throw new BusinessRuleException($"Cannot transition rental to {request.Status.ToString()} because inventory item is not available (current status: {inventoryItem.Status.ToString()})");
        }

        // Validate inventory item is available and not already reserved for the requested dates (for Booked)
        if (request.Status == RentalStatus.Booked)
        {
            var overlappingRentalExists = await dbContext.Rentals.AnyAsync(r =>
                r.InventoryItemId == entity.InventoryItemId &&
                r.Id != entity.Id &&
                r.Status != RentalStatus.Cancelled &&
                r.Status != RentalStatus.Returned &&
                r.StartDate <= entity.EndDate &&
                r.EndDate >= entity.StartDate,
                cancellationToken);

            if (overlappingRentalExists)
            {
                logger.LogError("Inventory item with ID {InventoryItemId} is already reserved for the selected dates", entity.InventoryItemId);
                throw new BusinessRuleException($"Inventory item with ID {entity.InventoryItemId} is already reserved for the selected dates");
            }

            await productApiClient.UpdateInventoryItemStatusAsync(entity.InventoryItemId, InventoryStatus.Reserved, $"Inventory item reserved for {request.Id}", cancellationToken);

        }

        if (request.Status == RentalStatus.PickedUp)
        {
            if (inventoryItem.Status != InventoryStatus.Available && inventoryItem.Status != InventoryStatus.Reserved)
            {
                logger.LogError("Inventory item with ID {InventoryItemId} is not available for booking", entity.InventoryItemId);
                throw new BusinessRuleException($"Inventory item with ID {entity.InventoryItemId} is not available for booking");
            }

            // Update inventory item status to Rented
            await productApiClient.UpdateInventoryItemStatusAsync(entity.InventoryItemId, InventoryStatus.Rented, $"Inventory item rented for {request.Id}, because it was booked", cancellationToken);
        }
        else if (request.Status == RentalStatus.Returned)
        {
            if (!request.ActualReturnDate.HasValue)
                throw new BusinessRuleException("ActualReturnDate is required when marking as Returned.");
            entity.ActualReturnDate = request.ActualReturnDate.Value;
        }

        if (request.Status == RentalStatus.Cancelled || request.Status == RentalStatus.Returned)
        {
            var hasFutureRental = await dbContext.Rentals.AnyAsync(r =>
                r.InventoryItemId == entity.InventoryItemId &&
                r.Status == RentalStatus.Booked &&
                r.StartDate > entity.ActualReturnDate,
                cancellationToken);
            var newStatus = hasFutureRental ? InventoryStatus.Reserved : InventoryStatus.Available;
            await productApiClient.UpdateInventoryItemStatusAsync(entity.InventoryItemId, newStatus, $"Inventory item returned from {request.Id}, status set to {newStatus}", cancellationToken);
        }


        entity.Status = request.Status;
        if (!string.IsNullOrWhiteSpace(request.Notes))
            entity.Notes = request.Notes;

        if (request.Status == RentalStatus.PickedUp)
        {
            if (!request.ActualStartDate.HasValue)
                throw new BusinessRuleException("ActualStartDate is required when marking as PickedUp.");
            entity.ActualStartDate = request.ActualStartDate.Value;
        }

        // Add RentalTimeline entry for status change
        dbContext.RentalTimelines.Add(new RentalService.Domain.Entities.RentalTimeline
        {
            Id = Guid.NewGuid(),
            RentalId = entity.Id,
            Status = (int)request.Status,
            Notes = request.Notes
        });

        // we need to handle the exception here and if exeption in creating rental, we need to rollback the inventory item status
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogDebug("Rental status updated for RentalId: {RentalId} to Status: {Status}", request.Id, request.Status);
        }
        catch (Exception ex)
        {
            if (request.Status == RentalStatus.Booked)
            {
                await productApiClient.UpdateInventoryItemStatusAsync(entity.InventoryItemId, InventoryStatus.Available, request.Notes, cancellationToken);
            }
            else if (request.Status == RentalStatus.Returned)
            {
                // On error, fallback to Available
                await productApiClient.UpdateInventoryItemStatusAsync(entity.InventoryItemId, InventoryStatus.Available, request.Notes, cancellationToken);
            }
            logger.LogError(ex, "Error occurred while updating rental status for RentalId: {RentalId}", request.Id);
            throw ex;
        }

        var rentalDto = await _mediator.Send(new GetRentalByIdQuery { Id = entity.Id }, cancellationToken);

        await _mediator.Send(new SendRentalNotificationCommand
        {
            Rental = new
            {
                rentalDto.RentalNumber,
                CustomerName = rentalDto.Customer?.Name,
                CustomerPhone = rentalDto.Customer?.PhoneNumber,
                ProductName = rentalDto.Product?.Name,
                rentalDto.Product.CategoryName,
                rentalDto.InventoryItem?.Size,
                rentalDto.InventoryItem?.Color,
                Status = rentalDto.Status.ToString(),
                Link = $"{_baseUrl.TrimEnd('/')}/rentals/details/{rentalDto.Id}"
            },
            Type = NotificationType.RentalStatusChanged
        });
    }

    private static bool IsValidTransition(RentalStatus current, RentalStatus next)
    {
        return current switch
        {
            RentalStatus.Pending => next == RentalStatus.Booked || next == RentalStatus.Cancelled,
            RentalStatus.Booked => next == RentalStatus.PickedUp || next == RentalStatus.Cancelled,
            RentalStatus.PickedUp => next == RentalStatus.Returned || next == RentalStatus.Overdue,
            RentalStatus.Overdue => next == RentalStatus.Returned,
            RentalStatus.Returned => false,
            RentalStatus.Cancelled => false,
            _ => false
        };
    }
}
