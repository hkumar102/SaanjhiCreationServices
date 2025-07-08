using MediatR;

namespace RentalService.Application.Rentals.Commands.Delete;

public class DeleteRentalCommand : IRequest
{
    public Guid Id { get; set; }
}