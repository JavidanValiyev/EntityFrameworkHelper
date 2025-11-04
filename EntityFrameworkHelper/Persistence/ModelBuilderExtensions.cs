using System.Linq.Expressions;
using EntityFrameworkHelper.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkHelper.Persistence;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// For adding global filters to queries
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="tenantId"></param>
    /// <typeparam name="TTenantIdType"></typeparam>
    public static void AddGlobalFilters<TTenantIdType>(this ModelBuilder builder, TTenantIdType tenantId)
        where TTenantIdType : struct, IComparable
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            var parameter = Expression.Parameter(clrType, "e");
            Expression? combinedFilter = null;

            // Tenant Filter
            if (typeof(ITenant<TTenantIdType>).IsAssignableFrom(clrType))
            {
                var tenantProperty = Expression.Property(parameter, nameof(ITenant<TTenantIdType>.TenantId));
                var tenantEquals = Expression.Equal(tenantProperty, Expression.Constant(tenantId));
                combinedFilter = Combine(combinedFilter, tenantEquals);
            }

            // Soft Delete Filter
            if (typeof(ISoftDeletable).IsAssignableFrom(clrType))
            {
                var isDeletedProperty = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var notDeleted = Expression.Not(isDeletedProperty);
                combinedFilter = Combine(combinedFilter, notDeleted);
            }

            if (combinedFilter == null) continue;
            var lambda = Expression.Lambda(combinedFilter, parameter);
            builder.Entity(clrType).HasQueryFilter(lambda);
        }
    }

    private static Expression? Combine(Expression? current, Expression next)
    {
        return current == null ? next : Expression.AndAlso(current, next);
    }
}

public static class DbContextExtensions
{
    public static void ConfigureStates<T>(this IEnumerable<EntityEntry>  entries,T tenantId) where T : struct,IComparable
    {
        foreach (var entry in entries)
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
                    // entity.CreatedBy =Guid.Parse(tenantId.ToString() ?? string.Empty);
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
            }
        }
    }
}