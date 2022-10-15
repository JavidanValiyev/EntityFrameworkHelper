using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkHelper.Persistence
{
    public abstract class ModelCacheKeyFactory<TTentantType> : IModelCacheKeyFactory where TTentantType : struct,IComparable
    {
        public object Create(DbContext context, bool designTime)
        => context is EfCoreHelperContext<TTentantType> dynamicContext
            ? (context.GetType(), dynamicContext.TenantId, designTime)
            : context.GetType();

        public object Create(DbContext context)
            => Create(context, true);
    }
}
