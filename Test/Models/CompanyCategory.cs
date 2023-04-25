using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkHelper.Contracts;

namespace Test.Models;

public class CompanyCategory : ITenant<Guid>
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("Tenant")]
    public Guid TenantId { get; set; }
    public Company Tenant { get; set; }
    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}