namespace EntityFrameworkHelper.Contracts;

public interface IContextUserManager<out T>
{ 
    T GetTenantId();
}