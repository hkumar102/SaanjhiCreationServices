using MediaService.Application.DTOs;
using MediaService.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace MediaService.Application.Interfaces;

public interface IMediaUploader
{
    Task<UploadMediaResult> UploadAsync(IFormFile file, MediaType mediaType, CancellationToken cancellationToken = default);
}