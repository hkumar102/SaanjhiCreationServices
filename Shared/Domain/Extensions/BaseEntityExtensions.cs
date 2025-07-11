using Shared.Domain.Entities;
using System;

namespace Shared.Domain.Extensions;

/// <summary>
/// Extension methods for BaseEntity
/// </summary>
public static class BaseEntityExtensions
{
    /// <summary>
    /// Soft deletes an entity
    /// </summary>
    /// <param name="entity">The entity to soft delete</param>
    /// <param name="deletedBy">User who performed the deletion</param>
    public static void SoftDelete(this BaseEntity entity, string? deletedBy = null)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = deletedBy;
    }

    /// <summary>
    /// Restores a soft deleted entity
    /// </summary>
    /// <param name="entity">The entity to restore</param>
    /// <param name="restoredBy">User who performed the restoration</param>
    public static void Restore(this BaseEntity entity, string? restoredBy = null)
    {
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.DeletedBy = null;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = restoredBy;
    }

    /// <summary>
    /// Checks if the entity is soft deleted
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <returns>True if soft deleted, false otherwise</returns>
    public static bool IsSoftDeleted(this BaseEntity entity)
    {
        return entity.IsDeleted;
    }

    /// <summary>
    /// Updates the audit fields for modification
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="modifiedBy">User who performed the modification</param>
    public static void MarkAsModified(this BaseEntity entity, string? modifiedBy = null)
    {
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = modifiedBy;
    }
}
