using MediatR;

namespace RentalService.Application.Rentals.Commands.Update;

public class UpdateRentalCommand : IRequest
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ShippingAddressId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public decimal RentalPrice { get; set; }
    public decimal SecurityDeposit { get; set; }

    public string? Height { get; set; }
    public string? Chest { get; set; }
    public string? Waist { get; set; }
    public string? Hip { get; set; }
    public string? Shoulder { get; set; }
    public string? SleeveLength { get; set; }
    public string? Inseam { get; set; }

    public string? Notes { get; set; }
}