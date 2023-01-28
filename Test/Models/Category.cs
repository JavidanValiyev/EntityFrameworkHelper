using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkHelper.Contracts.EntityContracts;

namespace Test.Models;

public class Category : ISoftDeletable,IAuditable<Guid>
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public Guid? CreatedBy { get; set; }
    public List<CompanyCategory> CompanyCategories { get; set; }
}