namespace EntityFrameworkHelper.Contracts.EntityContracts;

public interface IAuditable<TTenantIdType> : IBaseContract where TTenantIdType : struct, IComparable
{
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public TTenantIdType? ModifiedBy { get; set; }
    public TTenantIdType? CreatedBy { get; set; }
}