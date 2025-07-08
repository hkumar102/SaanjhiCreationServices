using CustomerService.Domain.Entities;
using MediatR;

namespace CustomerService.Application.Addresses.Commands.Queries.GetAddressesByCustomerId;

public class GetAddressesByCustomerIdQuery : IRequest<List<Address>>
{
    public Guid CustomerId { get; set; }
}