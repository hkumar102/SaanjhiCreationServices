namespace MediaService.Application.Commands;

using MediatR;
using Shared.Contracts.Media;


public class CreateMediaCommand : IRequest<MediaDto>
{
    public string FileName { get; set; } = null!;
    public string? Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
}