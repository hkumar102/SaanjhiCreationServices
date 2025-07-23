using System;

namespace RentalService.Contracts.DTOs;

public enum InventoryStatus
{
    /// <summary>
    /// Item is available for rental
    /// </summary>
    Available = 1,

    /// <summary>
    /// Item is currently rented out
    /// </summary>
    Rented = 2,

    /// <summary>
    /// Item is being cleaned after return
    /// </summary>
    Cleaning = 3,

    /// <summary>
    /// Item is under maintenance/repair
    /// </summary>
    Maintenance = 4,

    /// <summary>
    /// Item is damaged and cannot be rented
    /// </summary>
    Damaged = 5,

    /// <summary>
    /// Item is reserved for a future rental
    /// </summary>
    Reserved = 6,

    /// <summary>
    /// Item is in transit (shipping/receiving)
    /// </summary>
    InTransit = 7,

    /// <summary>
    /// Item is being inspected for quality
    /// </summary>
    Inspection = 8,

    /// <summary>
    /// Item is retired from service
    /// </summary>
    Retired = 9
}
public class RentalInventoryItemDto
{
   public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    // Physical Item Properties
    public string Size { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public string? BarcodeImageBase64 { get; set; }
    public string? QRCodeImageBase64 { get; set; } // QR code image as Base64 string
    // Item Status and Condition
    public InventoryStatus Status { get; set; }

}
