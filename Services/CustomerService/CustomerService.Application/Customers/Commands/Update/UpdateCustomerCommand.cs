using MediatR;

namespace CustomerService.Application.Customers.Commands.Update;

public class UpdateCustomerCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public Guid? UserId { get; set; }
}