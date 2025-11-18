using EntityFrameworkHelper.Persistence;
using NUnit.Framework;
using Test.Models;
using Test.Persistance;

namespace Test.Tests;


[TestFixture]
public class DbContextExtensionTest
{
    public IEnumerable<Company> GetSomeCompanies()
    {
        return Enumerable.Range(1, 5).Select(i => new Company()
        {
            Name = Faker.Name.FullName(),
        });
    }
    [Test]
    public void ConfigureStates_Should_Set_Auditable_Properties()
    {
        var tenantId = Guid.NewGuid();
        var dbContext = new AppDbContext();
        var company = GetSomeCompanies().First();
        dbContext.Companies.Add(company);
        dbContext.ChangeTracker.Entries().ConfigureStates(tenantId);
        Assert.AreEqual(company.CreatedBy,tenantId);
        Assert.AreEqual(company.CreatedDate.Date,DateTime.UtcNow.Date);
    }
    [Test]
    public void ConfigureStates_Should_Set_SoftDeletable_Properties()
    {
        var tenantId = Guid.NewGuid();
        var dbContext = new AppDbContext();
        var company = GetSomeCompanies().First();
        dbContext.Companies.Add(company);
        dbContext.SaveChanges();
        dbContext.Companies.Remove(company);
        dbContext.ChangeTracker.Entries().ConfigureStates(tenantId);
        Assert.IsTrue(company.IsDeleted);
        Assert.IsNotNull(company.DeletedDate);
    }
}