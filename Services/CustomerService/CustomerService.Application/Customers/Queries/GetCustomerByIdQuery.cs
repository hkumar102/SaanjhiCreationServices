using MediatR;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.Customers.Queries;

public class GetCustomerByIdQuery : IRequest<Customer?>
{
    public Guid Id { get; set; }
}