using Shared.Domain.Entities;

namespace MediaService.Domain.Entities;

public class Media : AuditableEntity
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
}
