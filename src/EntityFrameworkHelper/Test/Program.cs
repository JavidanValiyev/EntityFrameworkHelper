// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Test.Models;
using Test.Persistance;

Console.WriteLine("Hello, World!");

AppDbContext appDbContext = new AppDbContext();

var book = new Book()
{
    Name = "book 1"
};
appDbContext.Add(book);
appDbContext.SaveChanges();
var books = appDbContext.Books.ToList();

;