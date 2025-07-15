namespace MediaService.Application.Interfaces;

/// <summary>
/// File storage abstraction for storing processed images
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Save a file stream to storage and return the accessible URL
    /// </summary>
    /// <param name="fileStream">The file stream to save</param>
    /// <param name="fileName">The desired filename</param>
    /// <param name="contentType">The MIME content type</param>
    /// <returns>The URL where the file can be accessed</returns>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType);
    
    /// <summary>
    /// Delete a file from storage
    /// </summary>
    /// <param name="fileName">The filename to delete</param>
    Task DeleteFileAsync(string fileName);
    
    /// <summary>
    /// Check if a file exists in storage
    /// </summary>
    /// <param name="fileName">The filename to check</param>
    /// <returns>True if the file exists, false otherwise</returns>
    Task<bool> FileExistsAsync(string fileName);
}
