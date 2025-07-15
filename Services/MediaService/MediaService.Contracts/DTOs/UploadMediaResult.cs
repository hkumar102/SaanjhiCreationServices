using MediaService.Contracts.Enums;

namespace MediaService.Contracts.DTOs;

public class UploadMediaResult
{
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; } // Optional, useful for third-party deletes
    public MediaType MediaType { get; set; }
}
