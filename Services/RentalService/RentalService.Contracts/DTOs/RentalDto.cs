using RentalService.Contracts.Enums;

namespace RentalService.Contracts.DTOs;

public class RentalDto
{
    public Guid Id { get; set; }

    // References
    public Guid ProductId { get; set; }
    public Guid InventoryItemId { get; set; }
    public Guid CustomerId { get; set; }

    public RentalProductDto Product { get; set; } = null!;
    public RentalCustomerDto Customer { get; set; } = null!;
    public RentalInventoryItemDto InventoryItem { get; set; } = null!;
    public Guid ShippingAddressId { get; set; }
    public string? ShippingAddress { get; set; } // populated via CustomerApiClient

    // Rental Period
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }

    // Pricing
    public decimal RentalPrice { get; set; }
    public decimal DailyRate { get; set; }
    public decimal SecurityDeposit { get; set; }
    public decimal? LateFee { get; set; }
    public decimal? DamageFee { get; set; }

    // Status
    public RentalStatus Status { get; set; }

    // Measurements
    public string? Height { get; set; }
    public string? Chest { get; set; }
    public string? Waist { get; set; }
    public string? Hip { get; set; }
    public string? Shoulder { get; set; }
    public string? SleeveLength { get; set; }
    public string? Inseam { get; set; }

    // Business Properties
    public int BookNumber { get; set; }
    public string? Notes { get; set; }
    public string? ReturnConditionNotes { get; set; }
    public string RentalNumber { get; set; } = string.Empty;


    // Calculated Properties
    public int RentalDays { get; set; }
    public bool IsOverdue { get; set; }
    public decimal TotalAmount { get; set; }

    // Navigation Properties
    public ICollection<RentalTimelineDto> Timelines { get; set; } = new List<RentalTimelineDto>();
}