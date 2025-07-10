using MediatR;
using RentalService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace RentalService.Application.Rentals.Queries.GetRentals;

public class GetRentalsQuery : IRequest<PaginatedResult<RentalDto>>
{
    public List<Guid>? CustomerIds { get; set; }
    public List<Guid>? ProductIds { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string? SortBy { get; set; }
    public bool Descending { get; set; } = false;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}