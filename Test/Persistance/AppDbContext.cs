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
    public AppDbContext(ICurrentUserService<Guid> currentUserService) : base(currentUserService)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=localhost\MSSQLSERVER01;Database=efCoreTest;Trusted_Connection=True;");
        optionsBuilder.EnableSensitiveDataLogging(); 
    }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
}