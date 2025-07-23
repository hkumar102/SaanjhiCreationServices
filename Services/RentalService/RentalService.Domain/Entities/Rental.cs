using RentalService.Contracts.Enums;
using Shared.Domain.Entities;

namespace RentalService.Domain.Entities;

public class Rental : BaseEntity
{

    // Product & Inventory References
    public Guid ProductId { get; set; } // Reference to Product (catalog item)
    public Guid InventoryItemId { get; set; } // Reference to specific physical item being rented
    public Guid CustomerId { get; set; }

    // Rental Period
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualStartDate { get; set; } // When item was actually delivered/picked up
    public DateTime? ActualReturnDate { get; set; } // When item was actually returned

    // Pricing
    public decimal RentalPrice { get; set; } // Total rental price for the period
    public decimal DailyRate { get; set; } // Daily rental rate
    public decimal SecurityDeposit { get; set; }
    public decimal? LateFee { get; set; } // If returned late
    public decimal? DamageFee { get; set; } // If item was damaged

    // Logistics
    public Guid ShippingAddressId { get; set; }
    public RentalStatus Status { get; set; }

    // Customer Measurements (for proper fit)
    public string? Height { get; set; }
    public string? Chest { get; set; }
    public string? Waist { get; set; }
    public string? Hip { get; set; }
    public string? Shoulder { get; set; }
    public string? SleeveLength { get; set; }
    public string? Inseam { get; set; }


    // Business Properties
    public int BookNumber { get; set; }
    public string? Notes { get; set; } // Optional additional notes
    public string? ReturnConditionNotes { get; set; } // Condition when returned
    public string RentalNumber { get; set; } = string.Empty; // User-friendly reference number


    // Calculated Properties
    public int RentalDays => (int)(EndDate - StartDate).TotalDays + 1;
    public bool IsOverdue => DateTime.UtcNow > EndDate && Status != RentalStatus.Returned;
    public decimal TotalAmount => RentalPrice + SecurityDeposit + (LateFee ?? 0) + (DamageFee ?? 0);

    // Navigation Properties
    public ICollection<RentalTimeline> Timelines { get; set; } = new List<RentalTimeline>();
}