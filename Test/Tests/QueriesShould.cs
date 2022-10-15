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
using Test.Services;

namespace Test.Tests
{
    [TestFixture]
    public class QueriesShould
    {
        [Test]
        public void QueryResultsShould()
        {
            var tenantId = Guid.NewGuid();
            AppDbContext appDbContext = new AppDbContext(CurrentUserService.CreateFromManual(tenantId));
            
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
            Assert.AreEqual(tenantId, books.FirstOrDefault().Author.TenantId,
                "Author tenant Id should be same with book");

            Assert.IsTrue(bookName == "book" + tenantId);
        }
    }
}