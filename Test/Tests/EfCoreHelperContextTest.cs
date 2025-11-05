using NUnit.Framework;
using Test.Models;
using Test.Persistance;

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
    }
}