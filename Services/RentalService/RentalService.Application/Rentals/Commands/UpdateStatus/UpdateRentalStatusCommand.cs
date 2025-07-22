using MediatR;
using RentalService.Contracts.Enums;

namespace RentalService.Application.Rentals.Commands.UpdateStatus;

public class UpdateRentalStatusCommand : IRequest
{
    public Guid Id { get; set; }
    public RentalStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
}
