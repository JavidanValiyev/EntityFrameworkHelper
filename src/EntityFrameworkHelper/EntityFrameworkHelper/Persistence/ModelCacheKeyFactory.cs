using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkHelper.Persistence
{
    public class ModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
        => context is EfCoreHelperContext dynamicContext
            ? (context.GetType(), dynamicContext.TenantId, designTime)
            : (object)context.GetType();

        public object Create(DbContext context)
            => Create(context, true);
    }
}
