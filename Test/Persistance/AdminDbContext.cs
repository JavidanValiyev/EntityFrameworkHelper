using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Persistance;

public class AdminDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=localhost\MSSQLSERVER01;Database=efCoreTest;User Id=EfCoreTestUser;Password=Aa123456.!;Trusted_Connection=True;TrustServerCertificate=True;");
        optionsBuilder.EnableSensitiveDataLogging(); 
    }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CompanyCategory> CompanyCategories { get; set; }
}