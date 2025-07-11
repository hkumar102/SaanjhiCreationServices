using System;

namespace Shared.Domain.Entities;

/// <summary>
/// Base entity that provides common properties for all domain entities
/// </summary>
public abstract class BaseEntity : AuditableEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Indicates if the entity is soft deleted
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Timestamp when the entity was soft deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// User who performed the soft delete
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Version for optimistic concurrency control
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;
}
