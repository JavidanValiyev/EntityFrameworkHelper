namespace EntityFrameworkHelper.Contracts;

public interface ITenant<T> : IBaseContract where T :struct,IComparable
{
    public T TenantId { get; set; }
}