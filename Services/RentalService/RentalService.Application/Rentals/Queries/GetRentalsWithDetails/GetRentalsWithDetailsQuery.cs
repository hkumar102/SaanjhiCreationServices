
using MediatR;
using RentalService.Contracts.DTOs;
using Shared.Contracts.Common;
using RentalService.Domain.Entities;


namespace RentalService.Application.Rentals.Queries.GetRentalsWithDetails;

public class GetRentalsWithDetailsQuery : IRequest<List<RentalDto>>
{
    public IQueryable<Rental> Queryable { get; set; } = null!;
    public bool IncludeInventory { get; set; } = true;
    public bool IncludeCustomer { get; set; } = true;
    public bool IncludeProduct { get; set; } = true;
}
