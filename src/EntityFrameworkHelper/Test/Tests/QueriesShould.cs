﻿using Microsoft.AspNetCore.Http;
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
        [TestCase(20)]
        public void QueriResultShould(int testCount)
        {
            var serviceProvider = new ServiceCollection()
                                 .AddLogging()
                                 .AddScoped<IHttpContextAccessor, HttpContextAccessor>()
                                 .BuildServiceProvider();
            for (int i = 0; i < testCount; i++)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var httpAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                    httpAccessor.HttpContext = new DefaultHttpContext();
                    var tenantId = Guid.NewGuid().ToString();
                    httpAccessor.HttpContext.Request.Headers.Add("TenantId", tenantId);
                    AppDbContext appDbContext = new AppDbContext(httpAccessor);

                    var book = new Book()
                    {
                        Name = "book"+tenantId
                    };
                    appDbContext.Add(book);
                    appDbContext.SaveChanges();
                    Assert.IsFalse(book.Id == 0, "Book did not added !");
                    var books = appDbContext.Books.ToList();
                    Assert.IsFalse(!books.Any(), "Books not found!");
                    Assert.IsFalse(books.Count != 1, "Book count should be 1");
                    var bookName = books.FirstOrDefault().Name;
                    Assert.IsTrue(bookName == "book" + tenantId);
                }
            }

        }
    }
}
