using CustomerService.Contracts.DTOs;
using MediatR;

namespace CustomerService.Application.Customers.Queries;

public class GetCustomersByIdsQuery : IRequest<List<CustomerDto>>
{
    public List<Guid> CustomerIds { get; set; } = new();
}
