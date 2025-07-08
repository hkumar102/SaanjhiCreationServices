using MediatR;

namespace CustomerService.Application.Customers.Commands.Delete;

public class DeleteCustomerCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}