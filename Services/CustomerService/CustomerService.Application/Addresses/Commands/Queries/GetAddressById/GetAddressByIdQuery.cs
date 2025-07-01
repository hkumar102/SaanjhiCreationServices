using MediatR;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.Addresses.Queries.GetAddressById;

public class GetAddressByIdQuery : IRequest<Address?>
{
    public Guid Id { get; set; }
}