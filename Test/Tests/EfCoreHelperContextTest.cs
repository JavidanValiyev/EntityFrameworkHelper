using System.Linq.Expressions;
using System.Reflection;
using EntityFrameworkHelper.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUnit.Framework;
using Test.Models;
using Test.Persistance;
using Test.Services;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace Test.Tests
{
    
    [TestFixture]
    public class EfCoreHelperContextTest
    {

        public IEnumerable<Company> GetSomeCompanies()
        {
            return Enumerable.Range(1, 5).Select(i => new Company()
            {
                Name = Faker.Name.FullName(),
            });
        }
        [Test]
        public void AuditableEntityParametersCheck()
        {
            var createdBy = Guid.NewGuid();
            AppDbContext dbContext = new AppDbContext(createdBy);
            dbContext.Database.EnsureCreated();
            dbContext.Companies.Add(GetSomeCompanies().First());
            dbContext.SaveChanges();
            var company = dbContext.Companies.OrderByDescending(x=>x.CreatedDate).First();
            Assert.IsNotNull(company);
            Assert.IsNotNull(company.CreatedDate);
            Assert.AreEqual(company.CreatedBy,createdBy);
            Assert.IsFalse(company.IsDeleted);
            Assert.AreEqual(company.DeletedDate, null);
        }
        [Test]
        public void AuditableEntityParametersChecka()
        {
            var createdBy = Guid.NewGuid();
            AppDbContext dbContext = new AppDbContext(createdBy);
            dbContext.Database.EnsureCreated();
            dbContext.Companies.Add(GetSomeCompanies().First());
            dbContext.SaveChanges();
            dbContext.Companies.Remove(dbContext.Companies.First());
            dbContext.SaveChanges();
            Assert.AreEqual(0, dbContext.Companies.Count());
            Console.WriteLine(dbContext.Companies.Count());
        }
    }
}