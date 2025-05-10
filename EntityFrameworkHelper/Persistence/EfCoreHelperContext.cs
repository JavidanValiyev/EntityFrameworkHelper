using EntityFrameworkHelper.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EntityFrameworkHelper.Persistence
{
    /// <summary>
    /// You have to define your type of PK of your tenant table 
    /// </summary>
    /// <typeparam name="TTenantIdType"></typeparam>
    public abstract class EfCoreHelperContext<TTenantIdType> : DbContext where TTenantIdType : struct,IComparable
    {
        private List<Type> _types = new List<Type>();
        protected EfCoreHelperContext()
        {

        }

        protected EfCoreHelperContext(IContextUserManager<TTenantIdType> userManager)
        {
            _tenantId = userManager.GetTenantId();
        }
        private TTenantIdType _tenantId;

        public TTenantIdType TenantId { get => _tenantId;
            set => _tenantId = value;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, ModelCacheKeyFactory>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddGlobalFilters(_tenantId);
            base.OnModelCreating(modelBuilder);
        }

        private LambdaExpression CombineQueryFilters(Type entityType, LambdaExpression baseFilter, IEnumerable<LambdaExpression> andAlsoExpressions)
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
        public override int SaveChanges()
        {
            ConfigureStates();
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await Task.Run(ConfigureStates, cancellationToken); 
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void ConfigureStates()
        {
            foreach (var entry in ChangeTracker.Entries())
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
                    if (entry.Entity is IAuditable<TTenantIdType> entity)
                    {
                        entity.CreatedDate = DateTime.UtcNow;
                        entity.CreatedBy = _tenantId;
                    }
                    if (entry.Entity is ITenant<TTenantIdType> tenantEntity)
                    {
                        tenantEntity.TenantId = _tenantId;
                    }
                }
                else if (entry.Entity is IAuditable<TTenantIdType> entity && entry.State == EntityState.Modified)
                {
                    entity.LastModifiedDate = DateTime.UtcNow;
                    entity.ModifiedBy = _tenantId;
                }

            }
        }
    }
}
