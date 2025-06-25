using MediatR;

namespace UserService.Application.Commands;

public class UpdateUserProfileCommand : IRequest
{
    public string DisplayName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }
}