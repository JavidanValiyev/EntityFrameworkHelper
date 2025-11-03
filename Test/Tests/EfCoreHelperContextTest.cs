using System.Linq.Expressions;
using System.Reflection;
using EntityFrameworkHelper.Persistence;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Test.Models;
using Test.Persistance;
using Test.Services;

namespace Test.Tests
{
    [TestFixture]
    public class EfCoreHelperContextTest
    {
        private List<Category> GenerateCategoryList()
        {
            List<Category> categories = new List<Category>();

            for (int i = 0; i < 20; i++)
            {
                categories.Add(new Category()
                {
                    Name = Faker.Name.First()
                });
            }

            return categories;
        }

        private List<Company> GenerateCompany()
        {
            List<Company> companies = new List<Company>();
            for (int i = 0; i < 5; i++)
            {
                companies.Add(new Company()
                {
                    Name = Faker.Company.Name()
                });
            }

            return companies;
        }

        private void SetFakeDataToDatabase(AdminDbContext adminDbContext)
        {
            adminDbContext.Database.EnsureCreated();
            adminDbContext.Companies.AddRange(GenerateCompany());
            adminDbContext.Categories.AddRange(GenerateCategoryList());
            adminDbContext.SaveChanges();
        }

        [Test]
        public void CompaniesCountShouldNotBeOne()
        {
            AdminDbContext adminDbContext = new AdminDbContext();
            
            SetFakeDataToDatabase(adminDbContext);
            var tenantId = adminDbContext.Companies.FirstOrDefault()?.Id;
            Assert.AreNotEqual(null, tenantId);

            AppDbContext appDbContext = new(ContextUserManager.CreateFromManual(tenantId.GetValueOrDefault()));

            var companies = appDbContext.Companies.ToList();

            Assert.AreNotEqual(1, companies.Count());
        }

        [Test]
        public void CategoriesShouldOnlyOwn()
        {
            AdminDbContext adminDbContext = new AdminDbContext();
            SetFakeDataToDatabase(adminDbContext);
            Random random = new Random();
            var companyOne = adminDbContext.Companies.Skip(random.Next() % adminDbContext.Companies.Count())
                .FirstOrDefault();
            Assert.AreNotEqual(null, companyOne);
            var categories = adminDbContext.Categories.ToList();

            var assignedToCompanyOne = categories.Take(5).Select(c => new CompanyCategory()
            {
                TenantId = companyOne!.Id,
                CategoryId = c.Id
            }).ToList();

            companyOne!.CompanyCategories.AddRange(assignedToCompanyOne);
            adminDbContext.SaveChanges();

            AppDbContext appDbContext = new AppDbContext(ContextUserManager.CreateFromManual(companyOne.Id));

            var assignedCategoryList = appDbContext.Companies
                .Include(x => x.CompanyCategories)
                .FirstOrDefault(c => c.Id == companyOne.Id)?.CompanyCategories;
            Assert.AreNotEqual(null, assignedCategoryList);
            Assert.AreEqual(5, assignedCategoryList.Count(), "Assigned category list count should be 5");
        }
    }
}