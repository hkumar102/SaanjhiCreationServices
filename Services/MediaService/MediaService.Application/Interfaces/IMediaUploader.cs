using Microsoft.AspNetCore.Http;
using MediaService.Contracts.DTOs;
using MediaService.Contracts.Enums;

namespace MediaService.Application.Interfaces;

public interface IMediaUploader
{
    Task<UploadMediaResult> UploadAsync(IFormFile file, MediaType mediaType, CancellationToken cancellationToken = default);
}
