namespace CustomerService.Contracts.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public List<AddressDto> Addresses { get; set; } = new();
}