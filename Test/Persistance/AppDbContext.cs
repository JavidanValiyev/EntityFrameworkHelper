using EntityFrameworkHelper.Persistence;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkHelper.Contracts;
using Test.Models;

namespace Test.Persistance;

public class AppDbContext : EfCoreHelperContext<Guid>
{
    public AppDbContext()
    {

    }
    public AppDbContext(IContextUserManager<Guid> contextUserManager) : base(contextUserManager)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=localhost;Database=efCoreTest;Trusted_Connection=True;TrustServerCertificate=True;");
        optionsBuilder.EnableSensitiveDataLogging(); 
    }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CompanyCategory> CompanyCategories { get; set; }
}