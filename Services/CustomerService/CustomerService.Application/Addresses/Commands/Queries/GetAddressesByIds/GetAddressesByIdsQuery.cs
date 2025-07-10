using CustomerService.Contracts.DTOs;
using MediatR;

namespace CustomerService.Application.Addresses.Commands.Queries.GetAddressesByIds;

public class GetAddressesByIdsQuery : IRequest<List<AddressDto>>
{
    public List<Guid> AddressIds { get; set; } = new();
}
