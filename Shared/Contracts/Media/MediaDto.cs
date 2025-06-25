using System;

namespace Shared.Contracts.Media;

public class MediaDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
}
