using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using System;
using System.Linq;

namespace Shared.Infrastructure.Extensions;

/// <summary>
/// Extension methods for DbContext to handle BaseEntity audit fields
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Updates audit fields for all tracked entities before saving
    /// </summary>
    /// <param name="context">The DbContext</param>
    /// <param name="currentUser">Current user identifier</param>
    public static void UpdateAuditFields(this DbContext context, string? currentUser = null)
    {
        var entries = context.ChangeTracker.Entries<BaseEntity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = currentUser;
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = currentUser;
                    break;

                case EntityState.Modified:
                    // Don't update CreatedAt/CreatedBy on modifications
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = currentUser;
                    break;

                case EntityState.Deleted:
                    // Convert hard delete to soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = currentUser;
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = currentUser;
                    
                    // Don't update CreatedAt/CreatedBy on soft delete
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    break;
            }
        }
    }

    /// <summary>
    /// Gets all soft deleted entities of type T
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="context">The DbContext</param>
    /// <returns>Queryable of soft deleted entities</returns>
    public static IQueryable<T> GetSoftDeleted<T>(this DbContext context) where T : BaseEntity
    {
        return context.Set<T>().IgnoreQueryFilters().Where(e => e.IsDeleted);
    }

    /// <summary>
    /// Gets all entities including soft deleted ones
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="context">The DbContext</param>
    /// <returns>Queryable including soft deleted entities</returns>
    public static IQueryable<T> GetWithSoftDeleted<T>(this DbContext context) where T : BaseEntity
    {
        return context.Set<T>().IgnoreQueryFilters();
    }
}
