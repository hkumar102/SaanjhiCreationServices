using MediaService.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Media;

namespace MediaService.Application.Interfaces;

public interface IMediaUploader
{
    Task<UploadMediaResult> UploadAsync(IFormFile file, MediaType mediaType, CancellationToken cancellationToken = default);
}