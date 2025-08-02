using RentalService.Infrastructure.Persistence;
using RentalService.Domain.Entities;
using RentalService.Contracts.Enums;
using MediatR;
using RentalService.Application.Notifications.Commands;
using Microsoft.Extensions.Logging;
using NotificationService.Contracts.Enums;
using AutoMapper;
using RentalService.Contracts.DTOs;
using RentalService.Application.Rentals.Queries.GetRentalsWithDetails;
using System.Drawing;
using Microsoft.Extensions.Configuration;

namespace RentalService.Application.Jobs.Commands.AutoCancelPendingRentals;

public class AutoCancelPendingRentalsCommandHandler : IRequestHandler<AutoCancelPendingRentalsCommand>
{
    private readonly ILogger<AutoCancelPendingRentalsCommandHandler> _logger;
    private readonly RentalDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly string _baseUrl;

    public AutoCancelPendingRentalsCommandHandler(
        ILogger<AutoCancelPendingRentalsCommandHandler> logger,
        RentalDbContext dbContext,
        IMediator mediator,
        IMapper mapper,
        IConfiguration configuration)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mediator = mediator;
        _mapper = mapper;
        _baseUrl = configuration["App:AdminPortalBaseUrl"] ?? "https://www.saanjhicreation.com";
    }

    public async Task Handle(AutoCancelPendingRentalsCommand request, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("AutoCancelPendingRentalsJob");
        _logger.LogTrace("Starting auto-cancel job for pending rentals older than 5 days");
        var threshold = DateTime.UtcNow.Date.AddDays(-10);
        var pendingRentalsQueryable = _dbContext.Rentals
            .Where(r => r.Status == RentalStatus.Pending && r.CreatedAt < threshold);
        var pendingRentals = await _mediator.Send(new GetRentalsWithDetailsQuery { Queryable = pendingRentalsQueryable }, cancellationToken);
        _logger.LogInformation("Found {Count} pending rentals older than 5 days", pendingRentals.Count);
        foreach (var rental in pendingRentals)
        {
            _logger.LogTrace("Cancelling rental {RentalNumber} (Id: {RentalId})", rental.RentalNumber, rental.Id);
            rental.Status = RentalStatus.Cancelled;
            _dbContext.RentalTimelines.Add(new RentalTimeline
            {
                RentalId = rental.Id,
                Status = (int)RentalStatus.Cancelled,
                Notes = "Auto-cancelled by system after 2 days pending."
            });
            await _mediator.Send(new SendRentalNotificationCommand
            {
                Rental = new
                {
                    rental.RentalNumber,
                    CustomerName = rental.Customer?.Name,
                    CustomerPhone = rental.Customer?.PhoneNumber,
                    ProductName = rental.Product?.Name,
                    rental.Product.CategoryName,
                    rental.InventoryItem.Size,
                    rental.InventoryItem.Color,
                    Status = rental.Status.ToString(),
                    Link = $"{_baseUrl.TrimEnd('/')}/rentals/{rental.Id}"
                },
                Type = NotificationType.RentalStatusChanged
            }, cancellationToken);

            var rentalEntity = _dbContext.Rentals.Find(rental.Id);
            rentalEntity.Status = RentalStatus.Cancelled;
            _dbContext.Rentals.Update(rentalEntity);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Auto-cancel job completed. {Count} rentals cancelled.", pendingRentals.Count);
    }
}
