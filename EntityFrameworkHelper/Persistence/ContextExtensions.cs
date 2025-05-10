using System.Linq.Expressions;
using EntityFrameworkHelper.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkHelper.Persistence;

public static class ContextExtensions
{
    /// <summary>
    /// adding soft delete and tenant filter to your queries
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="tenantId"></param>
    /// <typeparam name="TTenantIdType"></typeparam>
    public static void AddGlobalFilters<TTenantIdType>(this ModelBuilder builder,TTenantIdType tenantId) where TTenantIdType : struct,IComparable
    {
        foreach (var type in builder.Model.GetEntityTypes())
        {
            var filters = new List<LambdaExpression>();
            var baseFilter = (Expression<Func<IBaseContract, bool>>)(_ => true);
            var interfaces = type.ClrType.GetInterfaces();
            if (interfaces.Contains(typeof(ITenant<TTenantIdType>)))
            {
                var tenantFilter = (Expression<Func<ITenant<TTenantIdType>, bool>>)(e => e.TenantId.Equals(tenantId));
                filters.Add(tenantFilter);
            }
            if (interfaces.Contains(typeof(ISoftDeletable)))
            {
                var softDeleteFilter = (Expression<Func<ISoftDeletable, bool>>)(e => !e.IsDeleted);
                filters.Add(softDeleteFilter);
            }
            var query = CombineQueryFilters(type.ClrType, baseFilter, filters);
            if(query != baseFilter)
                builder.Entity(type.ClrType).HasQueryFilter(query);
        }
    }
    /// <summary>
    /// adding soft delete pattern filter to queries
    /// </summary>
    /// <param name="builder"></param>
    public static void AddGlobalFilters(this ModelBuilder builder)
    {
        foreach (var type in builder.Model.GetEntityTypes())
        {
            var filters = new List<LambdaExpression>();
            var baseFilter = (Expression<Func<IBaseContract, bool>>)(_ => true);
            var interfaces = type.ClrType.GetInterfaces();
           if (interfaces.Contains(typeof(ISoftDeletable)))
            {
                var softDeleteFilter = (Expression<Func<ISoftDeletable, bool>>)(e => !e.IsDeleted);
                filters.Add(softDeleteFilter);
            }
            var query = CombineQueryFilters(type.ClrType, baseFilter, filters);
            if(query != baseFilter)
                builder.Entity(type.ClrType).HasQueryFilter(query);
        }
    }
    private static LambdaExpression CombineQueryFilters(Type entityType, LambdaExpression baseFilter, IEnumerable<LambdaExpression> andAlsoExpressions)
    {
        var newParam = Expression.Parameter(entityType ?? throw new ArgumentNullException(nameof(entityType)));

        if (baseFilter is null)
            throw new ArgumentNullException(nameof(baseFilter));
        var lambdaExpressions = andAlsoExpressions.ToList();
        if (!lambdaExpressions.Any())
            return baseFilter;
            
        var andAlsoExprBase = (Expression<Func<IBaseContract, bool>>)(_ => true);
        var expressionGlobal = ReplacingExpressionVisitor.Replace(andAlsoExprBase.Parameters.Single(), newParam, andAlsoExprBase.Body);
            
        foreach (var andAlso in lambdaExpressions)
        {
            var expression = ReplacingExpressionVisitor.Replace(andAlso.Parameters.Single(), newParam, andAlso.Body);
            expressionGlobal = Expression.AndAlso(expressionGlobal, expression);
        }

        var baseExp = ReplacingExpressionVisitor.Replace(baseFilter.Parameters.Single(), newParam, baseFilter.Body);
        var exp = Expression.AndAlso(baseExp, expressionGlobal);

        return Expression.Lambda(exp, newParam);
    }
}