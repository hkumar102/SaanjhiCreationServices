// File: Services/CustomerService/CustomerService.Application/Customers/Commands/CreateCustomerCommand.cs

using MediatR;

namespace CustomerService.Application.Customers.Commands.Create;

public class CreateCustomerCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public Guid? UserId { get; set; }
}