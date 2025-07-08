namespace RentalService.Contracts.DTOs;

public class RentalCustomerDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public List<RentalCustomerAddressDto> Addresses { get; set; } = new();
}