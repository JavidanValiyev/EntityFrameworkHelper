using EntityFrameworkHelper.Persistence;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkHelper.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Test.Models;

namespace Test.Persistance;

public class AppDbContext : DbContext
{
    private readonly Guid _tenantId;
    public AppDbContext(Guid tenantId)
    {
        _tenantId = tenantId;
    }
    public AppDbContext(IContextUserManager<Guid> contextUserManager)
    {
        _tenantId = contextUserManager.GetTenantId();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ReplaceService<IModelCacheKeyFactory, ModelCacheKeyFactory>();
        optionsBuilder.UseSqlServer(@"Server=localhost;Database=efCoreTest;Trusted_Connection=True;TrustServerCertificate=True;")
            .LogTo(Console.WriteLine, LogLevel.Information);;
        optionsBuilder.EnableSensitiveDataLogging(); 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddGlobalFilters(_tenantId);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ChangeTracker.Entries().ConfigureStates(_tenantId);
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await Task.Run(() =>
        {
            ChangeTracker.Entries().ConfigureStates(_tenantId);
        }, cancellationToken); 
        return await base.SaveChangesAsync(cancellationToken);    
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CompanyCategory> CompanyCategories { get; set; }
}