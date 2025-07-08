using MediatR;

namespace CustomerService.Application.Addresses.Commands.Delete;

public class DeleteAddressCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}