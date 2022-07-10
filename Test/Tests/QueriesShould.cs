using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models;
using Test.Persistance;

namespace Test.Tests
{
    [TestFixture]
    public class QueriesShould
    {
        [Test]
        public void QueryResultsShould()
        {
            var serviceProvider = new ServiceCollection()
                                 .AddLogging()
                                 .AddScoped<IHttpContextAccessor, HttpContextAccessor>()
                                 .BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var httpAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                httpAccessor.HttpContext = new DefaultHttpContext();
                var tenantId = Guid.NewGuid().ToString();
                httpAccessor.HttpContext.Request.Headers.Add("TenantId", tenantId);
                AppDbContext appDbContext = new AppDbContext(httpAccessor);

                var book = new Book()
                {
                    Name = "book" + tenantId,
                    Author = new Author()
                    {

                    }
                };
                appDbContext.Add(book);
                appDbContext.SaveChanges();
                Assert.IsFalse(book.Id == 0, "Book did not added !");
                var books = appDbContext.Books.Include(x => x.Author).Where(x => true);

                Console.WriteLine(books.ToQueryString());

                Assert.IsFalse(!books.ToList().Any(), "Books not found!");
                Assert.IsFalse(books.ToList().Count != 1, "Book count should be 1");
                var bookName = books.FirstOrDefault().Name;
                Assert.AreEqual(tenantId, books.FirstOrDefault().Author.TenantId.ToString(), "Author tenant Id should be same with book");

                Assert.IsTrue(bookName == "book" + tenantId);
            }
        }
    }
}
