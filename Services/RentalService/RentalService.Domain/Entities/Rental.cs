using RentalService.Contracts.Enums;
using Shared.Domain.Entities;

namespace RentalService.Domain.Entities;

public class Rental : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public Guid CustomerId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public decimal RentalPrice { get; set; }
    public decimal SecurityDeposit { get; set; }
    public Guid ShippingAddressId { get; set; }
    public RentalStatus Status { get; set; }

    // New: Measurement-related fields
    public string? Height { get; set; }
    public string? Chest { get; set; }
    public string? Waist { get; set; }
    public string? Hip { get; set; }
    public string? Shoulder { get; set; }
    public string? SleeveLength { get; set; }
    public string? Inseam { get; set; }

    public string? Notes { get; set; } // Optional additional notes
}