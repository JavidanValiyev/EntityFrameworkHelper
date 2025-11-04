using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkHelper.Persistence
{
    public abstract class ModelCacheKeyFactory<TTenantType> : IModelCacheKeyFactory where TTenantType : struct,IComparable
    {
        // public object Create(DbContext context, bool designTime)
        // => context is EfCoreHelperContext<TTenantType> dynamicContext
        //     ? (context.GetType(), dynamicContext.TenantId, designTime)
        //     : context.GetType();
        //
        // public object Create(DbContext context)
        //     => Create(context, true);
    }
}
