namespace EntityFrameworkHelper.Contracts;

public interface ICurrentUserService<out T>
{ 
    T GetTenantId();
}