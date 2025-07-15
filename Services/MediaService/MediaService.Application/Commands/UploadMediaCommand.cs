using MediaService.Contracts.DTOs;
using MediaService.Contracts.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MediaService.Application.Commands;

public class UploadMediaCommand : IRequest<UploadMediaResult>
{
    public IFormFile File { get; set; } = null!;
    public MediaType MediaType { get; set; }
}
