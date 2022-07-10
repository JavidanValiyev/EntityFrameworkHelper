using EntityFrameworkHelper.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models;

namespace Test.Persistance
{
    public class AppDbContext : EfCoreHelperContext
    {
        public AppDbContext(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"server=localhost;database=efCoreMultiTenant;user=sa;password=Aa123456!;Persist Security Info=True;Connect Timeout=300; Pooling=true; Max Pool Size=300");
            optionsBuilder.EnableSensitiveDataLogging(); 
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
