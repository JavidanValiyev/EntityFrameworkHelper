using System.Linq.Expressions;
using EntityFrameworkHelper.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkHelper.Persistence;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// The AddGlobalFilters method is an extension method for
    /// ModelBuilder that adds automatic filtering to
    /// database queries based on two common patterns:
    ///   * Multi-tenancy - Filtering data by tenant
    ///   * Soft deletion - Excluding soft-deleted records
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="tenantId">Id value of tenant</param>
    /// <typeparam name="TTenantIdType">Recommend to be Guid</typeparam>
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

