using MediatR;
using RentalService.Contracts.Enums;

namespace RentalService.Application.Rentals.Commands.Update;

public class UpdateRentalCommand : IRequest
{
    public Guid Id { get; set; } // Rental to update

    public Guid ProductId { get; set; }
    public Guid InventoryItemId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
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
    public decimal? LateFee { get; set; }
    public decimal? DamageFee { get; set; }
    public string? ReturnConditionNotes { get; set; }
    // Receipt Information
    public string? ReceiptDocumentUrl { get; set; } // URL to the receipt document
    // Optionally, you may want to allow updating Status:
    public RentalStatus Status { get; set; }
}