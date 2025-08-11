using Imagekit.Sdk;
using MediaService.Application.Commands;
using MediaService.Application.Interfaces;
using MediaService.Contracts.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediaService.Application.Handlers;

public class UploadMediaCommandHandler(ImagekitClient imagekitClient, IImageUtlityService imageUtlityService, ILogger<UploadMediaCommandHandler> logger)
    : IRequestHandler<UploadMediaCommand, UploadMediaResult>
{
    public async Task<UploadMediaResult> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        var optimizedImage = await imageUtlityService.CompressImage(request.File);
        
        var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";

        var uploadRequest = new FileCreateRequest
        {
            file = optimizedImage.data,
            fileName = uniqueFileName,
            folder = $"/{request.FolderName}/",
            useUniqueFileName = false,
            isPrivateFile = false,
        };
        
        var result = await imagekitClient.UploadAsync(uploadRequest);
        
        if (result == null || string.IsNullOrEmpty(result.url))
        {
            logger.LogError("ImageKit upload failed - no URL returned for file: {FileName}", result?.Raw);
            throw new InvalidOperationException("ImageKit upload failed - no URL returned");
        }

        logger.LogDebug("ImageKit upload successful: {Url}", result.url);
        return new UploadMediaResult
        {
            Url = result.url,
            PublicId = result.fileId,
            MediaType = request.MediaType
        };
    }
}
