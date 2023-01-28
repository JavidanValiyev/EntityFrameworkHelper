namespace EntityFrameworkHelper.Contracts.EntityContracts;

public interface ITenant<T> : IBaseContract where T :struct,IComparable
{
    public T TenantId { get; set; }
}