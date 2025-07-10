using AutoMapper;
using CustomerService.Contracts.DTOs;
using CustomerService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Commands.Queries.GetAddressesByIds;

public class GetAddressesByIdsQueryHandler(CustomerDbContext context, IMapper mapper) 
    : IRequestHandler<GetAddressesByIdsQuery, List<AddressDto>>
{
    public async Task<List<AddressDto>> Handle(GetAddressesByIdsQuery request, CancellationToken cancellationToken)
    {
        if (!request.AddressIds.Any())
            return new List<AddressDto>();

        var addresses = await context.Addresses
            .Where(a => request.AddressIds.Contains(a.Id))
            .ToListAsync(cancellationToken);

        return mapper.Map<List<AddressDto>>(addresses);
    }
}
