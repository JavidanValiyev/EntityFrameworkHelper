using EntityFrameworkHelper.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkHelper.Persistence
{
    public abstract class EfCoreHelperContext : DbContext
    {
        private List<Type> _types = null;
        public EfCoreHelperContext(IHttpContextAccessor httpContextAccessor,string tenantKey = "TenantId")
        {
            _tenantId = Guid.Parse(httpContextAccessor.HttpContext.Request.Headers[tenantKey].ToString());
        }
        private Guid _tenantId;

        public Guid TenantId { get { return _tenantId; } set { if (value != default) _tenantId = value; } }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, EntityFrameworkHelper.Persistence.ModelCacheKeyFactory>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var types = GetEntityTypes();
            foreach (var type in GetEntityTypes())
            {
                var filters = new List<LambdaExpression>();
                var baseFilter = (Expression<Func<IBaseContract, bool>>)(_ => true);
                var interfaces = type.GetInterfaces();
                if (interfaces.Contains(typeof(ITenant)))
                {
                    var tenantFilter = (Expression<Func<ITenant, bool>>)(e => e.TenantId == _tenantId);
                    filters.Add(tenantFilter);
                }
                if (interfaces.Contains(typeof(ISoftDeletable)))
                {
                    var softDeleteFilter = (Expression<Func<ISoftDeletable, bool>>)(e => !e.IsDeleted);
                    filters.Add(softDeleteFilter);
                }
                var query = CombineQueryFilters(type, baseFilter, filters);
                modelBuilder.Entity(type).HasQueryFilter(query);
            }
            base.OnModelCreating(modelBuilder);
        }
        private LambdaExpression CombineQueryFilters(Type entityType, LambdaExpression baseFilter, IEnumerable<LambdaExpression> andAlsoExpressions)
        {
            var newParam = Expression.Parameter(entityType);
            var andAlsoExprBase = (Expression<Func<IBaseContract, bool>>)(_ => true);
            var expressionGlobal = ReplacingExpressionVisitor.Replace(andAlsoExprBase.Parameters.Single(), newParam, andAlsoExprBase.Body);
            foreach (var andAlso in andAlsoExpressions)
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
            await Task.Run(() =>
            {
                ConfigureStates();
            }); 
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
                    if ((entry.Entity is IAuditable))
                    {
                        var entity = (IAuditable)entry.Entity;
                        entity.CreatedDate = DateTime.UtcNow;
                        entity.CreatedBy = _tenantId;
                    }
                    if (entry.Entity is ITenant)
                    {
                        var tenantEntity = (ITenant)entry.Entity;
                        tenantEntity.TenantId = _tenantId;
                    }
                }
                else if (entry.Entity is IAuditable && entry.State == EntityState.Modified)
                {
                    var entity = (IAuditable)entry.Entity;
                    entity.LastModifiedDate = DateTime.UtcNow;
                    entity.ModifiedBy = _tenantId;
                }

            }
        }
        private IEnumerable<Type> GetEntityTypes()
        {
            if (_types is not null && _types.Any())
                return _types;

            List<Type> result = new List<Type>();
            foreach (var properties in this.GetType().GetProperties())
            {
                if (properties.PropertyType.IsGenericType && properties.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    var type = properties.PropertyType.GetGenericArguments().FirstOrDefault();
                    result.Add(type);
                }
            }
            return result;
        }
    }
}
