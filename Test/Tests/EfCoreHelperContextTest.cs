using NUnit.Framework;
using Test.Models;
using Test.Persistance;

namespace Test.Tests
{
    
    [TestFixture]
    public class EfCoreHelperContextTest
    {
        private AppDbContext _dbContext;
        private Guid _createdBy;
        
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
            _dbContext = new AppDbContext(_createdBy);
            _dbContext.Companies.Add(GetSomeCompanies().First());
            _dbContext.SaveChanges();
            var company = _dbContext.Companies.OrderByDescending(x=>x.CreatedDate).First();
            Assert.IsNotNull(company);
            Assert.IsNotNull(company.CreatedDate);
            Assert.AreEqual(company.CreatedBy,_createdBy);
            Assert.IsFalse(company.IsDeleted);
            Assert.AreEqual(company.DeletedDate, null);
        }
        [SetUp]
        public void RunBeforeAllTestsInFixture()
        {
            Guid createdBy = Guid.NewGuid();
            _dbContext = new AppDbContext(isAdminContext: true, passStateConfigurations:true);
            _createdBy = createdBy;
            _dbContext.Database.EnsureCreated();
            _dbContext.Companies.RemoveRange(_dbContext.Companies);
            _dbContext.SaveChanges();
            Console.WriteLine("OneTimeSetUp for MyTests executed.");
        }
        // ReSharper disable once NUnit.TestCaseSourceMustBeStatic
        [Test]
        public void DeleteEntity()
        {
            _dbContext = new AppDbContext(_createdBy);
            var company = GetSomeCompanies().First();
            _dbContext.Database.EnsureCreated();
            _dbContext.Companies.Add(company);
            _dbContext.SaveChanges();
            _dbContext.Companies.Remove(company);
            _dbContext.SaveChanges();
            Assert.AreEqual(0,_dbContext.Companies.Count());
        }
    }
}