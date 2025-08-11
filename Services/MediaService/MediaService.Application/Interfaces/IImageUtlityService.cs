namespace MediaService.Application.Interfaces;

public interface IImageUtlityService
{
    Task<(byte[] data, string contentType, int width, int height)> CompressImage(Microsoft.AspNetCore.Http.IFormFile file);
}