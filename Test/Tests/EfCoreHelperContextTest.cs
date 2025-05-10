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

        private void SetFakeDatToDatabase(AdminDbContext adminDbContext)
        {
            adminDbContext.Companies.AddRange(GenerateCompany());
            adminDbContext.Categories.AddRange(GenerateCategoryList());
            adminDbContext.SaveChanges();
        }

        [Test]
        public void CompaniesCountShouldNotBeOne()
        {
            AdminDbContext adminDbContext = new AdminDbContext();
            SetFakeDatToDatabase(adminDbContext);
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
            SetFakeDatToDatabase(adminDbContext);
            Random random = new Random();
            var companyOne = adminDbContext.Companies.Skip(random.Next() % adminDbContext.Companies.Count())
                .FirstOrDefault();
            Assert.AreNotEqual(null, companyOne);
            var categories = adminDbContext.Categories.ToList();

            var assignedToCompanyOne = categories.Take(5).Select(c => new CompanyCategory()
            {
                TenantId = companyOne.Id,
                CategoryId = c.Id
            }).ToList();

            companyOne.CompanyCategories.AddRange(assignedToCompanyOne);
            adminDbContext.SaveChanges();

            AppDbContext appDbContext = new AppDbContext(ContextUserManager.CreateFromManual(companyOne.Id));

            var assignedCategoryList = appDbContext.Companies
                .Include(x => x.CompanyCategories)
                .FirstOrDefault(c => c.Id == companyOne.Id)?.CompanyCategories;
            Assert.AreNotEqual(null, assignedCategoryList);
            Assert.AreEqual(5, assignedCategoryList.Count(), "Assigned category list count should be 5");
        }

        [SetUp]
        public static void AssemblyInit()
        {
            // AdminDbContext adminDbContext = new AdminDbContext();
            // adminDbContext.CompanyCategories.RemoveRange(adminDbContext.CompanyCategories.ToList());
            // adminDbContext.Categories.RemoveRange(adminDbContext.Categories.ToList());
            // adminDbContext.Companies.RemoveRange(adminDbContext.Companies.ToList());
            // adminDbContext.SaveChanges();
        }
    }


    public class QueryFilterTests
    {
        // Define the entity type for testing
        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // Test case for CombineQueryFilters
        [Test]
        public void TestCombineQueryFilters()
        {
            // Arrange
            Type entityType = typeof(TestEntity);

            // Create the base filter expression
            Expression<Func<TestEntity, bool>> baseFilter = e => e.Id > 0;

            // Create a list of additional filter expressions
            List<Expression<Func<TestEntity, bool>>> andAlsoExpressions = new List<Expression<Func<TestEntity, bool>>>
            {
                e => e.Name.Contains("John"),
                e => e.Id < 100
            };

            // Create an instance of the class containing the CombineQueryFilters method
            var queryFilter = new AppDbContext();

            // Act
            var result = queryFilter.CombineQueryFilters(entityType, baseFilter, andAlsoExpressions);

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<LambdaExpression>(result);
        }

        [Test]
        public void TestCombineQueryFilters_NullEntityType_ThrowsArgumentNullException()
        {
            // Arrange
            Type entityType = null;
            Expression<Func<TestEntity, bool>> baseFilter = e => e.Id > 0;
            List<Expression<Func<TestEntity, bool>>>
                andAlsoExpressions = new List<Expression<Func<TestEntity, bool>>>();

            var queryFilter = new AppDbContext();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                queryFilter.CombineQueryFilters(entityType, baseFilter, andAlsoExpressions));
        }

        [Test]
        public void TestCombineQueryFilters_NullBaseFilter_ThrowsArgumentNullException()
        {
            // Arrange
            Type entityType = typeof(TestEntity);
            Expression<Func<TestEntity, bool>> baseFilter = null;
            List<Expression<Func<TestEntity, bool>>>
                andAlsoExpressions = new List<Expression<Func<TestEntity, bool>>>();

            var queryFilter = new AppDbContext();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                queryFilter.CombineQueryFilters(entityType, baseFilter, andAlsoExpressions));
        }

        [Test]
        public void TestCombineQueryFilters_NullAndAlsoExpressions_ReturnsBaseFilter()
        {
            // Arrange
            Type entityType = typeof(TestEntity);
            Expression<Func<TestEntity, bool>> baseFilter = e => e.Id > 0;
            List<Expression<Func<TestEntity, bool>>> andAlsoExpressions = null;

            var queryFilter = new AppDbContext();

            // Act
            var result = queryFilter.CombineQueryFilters(entityType, baseFilter, andAlsoExpressions);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(baseFilter, result);
        }

        [Test]
        public void TestCombineQueryFilters_EmptyAndAlsoExpressions_ReturnsBaseFilter()
        {
            // Arrange
            Type entityType = typeof(TestEntity);
            Expression<Func<TestEntity, bool>> baseFilter = e => e.Id > 0;
            List<Expression<Func<TestEntity, bool>>>
                andAlsoExpressions = new List<Expression<Func<TestEntity, bool>>>();

            var queryFilter = new AppDbContext();

            // Act
            var result = queryFilter.CombineQueryFilters(entityType, baseFilter, andAlsoExpressions);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(baseFilter, result);
        }
    }
}