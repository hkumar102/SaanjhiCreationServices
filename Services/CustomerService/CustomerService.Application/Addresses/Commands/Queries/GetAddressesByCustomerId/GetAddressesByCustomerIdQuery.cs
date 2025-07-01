using MediatR;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.Addresses.Queries.GetAddressesByCustomerId;

public class GetAddressesByCustomerIdQuery : IRequest<List<Address>>
{
    public Guid CustomerId { get; set; }
}