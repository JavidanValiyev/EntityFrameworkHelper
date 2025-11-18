using EntityFrameworkHelper.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Test.Models;

namespace Test.Persistance;

public class AppDbContext : DbContext
{
    private readonly Guid _tenantId;
    private readonly bool _isAdminContext = false;
    private readonly bool _passStateConfigurations = false;
    public AppDbContext(Guid tenantId = default,bool isAdminContext = false,bool passStateConfigurations = false)
    {
        _passStateConfigurations = passStateConfigurations;
        _isAdminContext = isAdminContext;
        _tenantId = tenantId;
    }
    public AppDbContext()
    {
        _tenantId = Guid.NewGuid();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(_isAdminContext)
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, ModelCacheKeyFactory>();
        optionsBuilder.UseSqlServer(@"Server=localhost;Database=efCoreTest;Trusted_Connection=True;TrustServerCertificate=True;")
            .LogTo(Console.WriteLine, LogLevel.Information);;
        optionsBuilder.EnableSensitiveDataLogging(); 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if(!_isAdminContext)
            modelBuilder.AddGlobalFilters(_tenantId);
        base.OnModelCreating(modelBuilder);
    }
    
    public override int SaveChanges()
    {
        if(!_passStateConfigurations)
            ChangeTracker.Entries().ConfigureStates(_tenantId);
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        if(!_passStateConfigurations)
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