
using UserService.Application.Commands;

namespace UserService.Application.Mappings;

using AutoMapper;
using Domain.Entities;
using Shared.Contracts.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserRole, UserRoleDto>();
        CreateMap<ShippingAddress, ShippingAddressDto>();
        CreateMap<AuthProvider, AuthProviderDto>();
        CreateMap<CreateUserCommand, User>();

        // Optional: Reverse mappings if needed
        CreateMap<UserDto, User>();
        CreateMap<UserRoleDto, UserRole>();
        CreateMap<ShippingAddressDto, ShippingAddress>();
        CreateMap<AuthProviderDto, AuthProvider>();
    }
}