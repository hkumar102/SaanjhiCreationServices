using MediaService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Media;

namespace MediaService.Application.Commands;

public class UploadMediaCommand : IRequest<UploadMediaResult>
{
    public IFormFile File { get; set; } = null!;
    public MediaType MediaType { get; set; }
}
