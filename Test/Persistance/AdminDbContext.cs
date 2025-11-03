using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Persistance;

public class AdminDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=localhost;Database=efCoreTest;Trusted_Connection=True;TrustServerCertificate=True;");
        optionsBuilder.EnableSensitiveDataLogging(); 
        
    }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CompanyCategory> CompanyCategories { get; set; }
}