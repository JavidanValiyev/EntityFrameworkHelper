using EntityFrameworkHelper.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkHelper.Persistence;

public static class DbContextExtensions
{
    public static void ConfigureStates<T>(this IEnumerable<EntityEntry>  entries,T tenantId) where T : struct,IComparable
    {
        foreach (var entry in entries.Where(x=>x.Entity is IAuditable<T> or ITenant<T> or ISoftDeletable))
        {
            if (entry.State == EntityState.Deleted & entry.Entity is ISoftDeletable)
            {
                entry.State = EntityState.Modified;
                var entity = (ISoftDeletable)entry.Entity;
                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Added)
            {
                if (entry.Entity is IAuditable<Guid> entity)
                {
                    entity.CreatedDate = DateTime.UtcNow;
                    entity.CreatedBy =Guid.Parse(tenantId.ToString() ?? string.Empty);
                }
                if (entry.Entity is ITenant<T> tenantEntity)
                {
                    tenantEntity.TenantId = tenantId;
                }
            }
            else if (entry.Entity is IAuditable<T> entity && entry.State == EntityState.Modified)
            {
                entity.LastModifiedDate = DateTime.UtcNow;
                entity.ModifiedBy =tenantId;
                entry.Property(nameof(entity.CreatedBy)).IsModified = false;
                entry.Property(nameof(entity.CreatedDate)).IsModified = false;
            }
        }
    }
}