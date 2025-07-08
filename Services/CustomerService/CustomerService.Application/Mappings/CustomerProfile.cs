using AutoMapper;
using CustomerService.Contracts.DTOs;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.Mappings;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses));
        CreateMap<Address, AddressDto>();
    }
}