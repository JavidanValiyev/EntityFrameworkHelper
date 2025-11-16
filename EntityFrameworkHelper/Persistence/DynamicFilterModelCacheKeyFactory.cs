using EntityFrameworkHelper.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkHelper.Persistence;

/// <summary>
/// For update dynamic filters by tenant
/// </summary>
public class DynamicFilterModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        if (designTime) return this;
        var key = new ModelCacheKey(context, designTime);

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (context is not IDynamicQueryFilterSource dynamicSource)
            return key;
        return (key, dynamicSource.FilterKey);
    }
}