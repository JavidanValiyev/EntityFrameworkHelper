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
        var key = new ModelCacheKey(context, designTime);

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (context is IDynamicQueryFilterSource dynamicSource)
        {
            return (key, dynamicSource.FilterKey);
        }

        return key;
    }
}