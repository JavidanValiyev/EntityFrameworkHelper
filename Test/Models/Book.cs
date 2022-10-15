using EntityFrameworkHelper.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class Book : ISoftDeletable,IAuditable<Guid>,ITenant<Guid>
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid TenantId { get; set; }
        [ForeignKey("Author")]
        public int? AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
