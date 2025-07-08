using MediatR;
using RentalService.Contracts.DTOs;

namespace RentalService.Application.Rentals.Queries.GetRentalById;

public class GetRentalByIdQuery : IRequest<RentalDto>
{
    public Guid Id { get; set; }
}