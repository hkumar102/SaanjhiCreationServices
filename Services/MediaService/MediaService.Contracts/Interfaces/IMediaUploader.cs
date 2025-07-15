using Microsoft.AspNetCore.Http;
using MediaService.Contracts.DTOs;
using MediaService.Contracts.Enums;

namespace MediaService.Contracts.Interfaces;

public interface IMediaUploader
{
    Task<UploadMediaResult> UploadAsync(IFormFile file, MediaType mediaType, CancellationToken cancellationToken = default);
}
