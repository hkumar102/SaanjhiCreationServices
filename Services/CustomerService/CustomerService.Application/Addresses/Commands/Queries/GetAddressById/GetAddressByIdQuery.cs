using CustomerService.Domain.Entities;
using MediatR;

namespace CustomerService.Application.Addresses.Commands.Queries.GetAddressById;

public class GetAddressByIdQuery : IRequest<Address?>
{
    public Guid Id { get; set; }
}