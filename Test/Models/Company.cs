using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkHelper.Contracts;

namespace Test.Models;

public class Company  
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<CompanyCategory> CompanyCategories { get; set; } = new List<CompanyCategory>();
}