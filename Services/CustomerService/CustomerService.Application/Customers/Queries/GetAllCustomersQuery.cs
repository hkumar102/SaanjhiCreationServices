using CustomerService.Contracts.DTOs;
using MediatR;
using Shared.Contracts.Common;

namespace CustomerService.Application.Customers.Queries;

public class GetAllCustomersQuery : IRequest<PaginatedResult<CustomerDto>>
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}