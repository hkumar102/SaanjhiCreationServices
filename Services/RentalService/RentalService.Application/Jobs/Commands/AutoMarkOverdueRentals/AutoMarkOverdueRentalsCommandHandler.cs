using RentalService.Infrastructure.Persistence;
using RentalService.Domain.Entities;
using RentalService.Contracts.Enums;
using MediatR;
using RentalService.Application.Notifications.Commands;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using NotificationService.Contracts.Enums;
using RentalService.Contracts.DTOs;
using RentalService.Application.Rentals.Queries.GetRentalsWithDetails;

namespace RentalService.Application.Jobs.Commands.AutoMarkOverdueRentals;

public class AutoMarkOverdueRentalsCommandHandler : IRequestHandler<AutoMarkOverdueRentalsCommand>
{
    private readonly ILogger<AutoMarkOverdueRentalsCommandHandler> _logger;
    private readonly RentalDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly string _baseUrl;

    public AutoMarkOverdueRentalsCommandHandler(
        ILogger<AutoMarkOverdueRentalsCommandHandler> logger,
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

    public async Task Handle(AutoMarkOverdueRentalsCommand request, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("AutoMarkOverdueRentalsJob");
        _logger.LogTrace("Starting auto-mark overdue job for rentals past end date");
        var now = DateTime.UtcNow.Date;
        var overdueRentalsQueryable = _dbContext.Rentals
            .Where(r => r.Status == RentalStatus.PickedUp && r.EndDate < now);
        var overdueRentals = await _mediator.Send(new GetRentalsWithDetailsQuery { Queryable = overdueRentalsQueryable }, cancellationToken);
        _logger.LogInformation("Found {Count} rentals past end date and not returned/cancelled", overdueRentals.Count);
        foreach (var rental in overdueRentals)
        {
            _logger.LogTrace("Marking rental {RentalNumber} (Id: {RentalId}) as Overdue", rental.RentalNumber, rental.Id);
            rental.Status = RentalStatus.Overdue;
            _dbContext.RentalTimelines.Add(new RentalTimeline
            {
                RentalId = rental.Id,
                Status = (int)RentalStatus.Overdue,
                Notes = "Auto-marked as overdue by system."
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
                    Link = $"{_baseUrl.TrimEnd('/')}/rentals/manage/{rental.Id}"
                },
                Type = NotificationType.RentalStatusChanged
            }, cancellationToken);

            var rentalEntity = _dbContext.Rentals.Find(rental.Id);
            rentalEntity.Status = RentalStatus.Overdue;
            _dbContext.Rentals.Update(rentalEntity);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Auto-mark overdue job completed. {Count} rentals marked as overdue.", overdueRentals.Count);
    }
}
