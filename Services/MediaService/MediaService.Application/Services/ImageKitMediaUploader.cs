using System.Net.Http.Headers;
using Imagekit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shared.Contracts.Media;
using MediaService.Application.DTOs;
using MediaService.Application.Interfaces;

namespace MediaService.Application.Services;

public class ImageKitMediaUploader(IConfiguration config) : IMediaUploader
{
    public async Task<UploadMediaResult> UploadAsync(IFormFile file, MediaType mediaType,
        CancellationToken cancellationToken = default)
    {
        var privateKey = config["ImageKit:PrivateKey"];
        var publicKey = config["ImageKit:PublicKey"];
        var urlEndpoint = config["ImageKit:UrlEndpoint"] ?? "https://ik.imagekit.io/your_imagekit_id/";
        var imagekitId = config["ImageKit:ImagekitId"];
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream, cancellationToken);
            Imagekit.Sdk.ImagekitClient imagekitClient =
                new Imagekit.Sdk.ImagekitClient(publicKey, privateKey, urlEndpoint);
            var fileCreateRequest = new FileCreateRequest
            {
                file = memoryStream.ToArray(),
                overwriteFile = true,
                fileName = file.FileName,
                useUniqueFileName = true
            };
            var result = await imagekitClient.UploadAsync(fileCreateRequest);
            
            return new UploadMediaResult
            {
                Url = result.url,
                PublicId = result.fileId,
                MediaType = mediaType
            };
        }
    }
}