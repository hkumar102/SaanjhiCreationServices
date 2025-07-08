using CustomerService.Contracts.DTOs;
using MediatR;

namespace CustomerService.Application.Customers.Queries;

public class GetCustomerByIdQuery : IRequest<CustomerDto?>
{
    public Guid Id { get; set; }
}