using MediatR;

namespace RentalService.Application.Rentals.Commands.Create;

public class CreateRentalCommand : IRequest<Guid>
{
    public Guid ProductId { get; set; }
    public Guid InventoryItemId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal RentalPrice { get; set; }
    public decimal DailyRate { get; set; }
    public decimal SecurityDeposit { get; set; }
    public Guid ShippingAddressId { get; set; }
    public int BookNumber { get; set; }
    public string? Notes { get; set; }
    public string? Height { get; set; }
    public string? Chest { get; set; }
    public string? Waist { get; set; }
    public string? Hip { get; set; }
    public string? Shoulder { get; set; }
    public string? SleeveLength { get; set; }
    public string? Inseam { get; set; }
    public string? RentalNumber { get; set; } // Optional, can be generated if not provided

    // Optionally include these if needed:
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public decimal? LateFee { get; set; }
    public decimal? DamageFee { get; set; }
    public string? ReturnConditionNotes { get; set; }
}