namespace RentalService.Contracts.DTOs;

public class RentalDto
{
    public Guid Id { get; set; }

    public RentalProductDto Product { get; set; } = null!;
    public RentalCustomerDto Customer { get; set; } = null!;
    public Guid ShippingAddressId { get; set; }
    public string? ShippingAddress { get; set; } // populated via CustomerApiClient
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public decimal RentalPrice { get; set; }
    public decimal SecurityDeposit { get; set; }
    public string Status { get; set; } = null!;

    // Measurements
    public string? Height { get; set; }
    public string? Chest { get; set; }
    public string? Waist { get; set; }
    public string? Hip { get; set; }
    public string? Shoulder { get; set; }
    public string? SleeveLength { get; set; }
    public string? Inseam { get; set; }

    public string? Notes { get; set; }
}