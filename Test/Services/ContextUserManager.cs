using EntityFrameworkHelper.Contracts;

namespace Test.Services;

public class ContextUserManager : IContextUserManager<Guid>
{
    private ContextUserManager(Guid Id)
    {
        this.Id = Id;
    }
    private Guid Id { get; }
    public Guid GetTenantId()
    {
        if(Id != default)
            return Id;
        return  Guid.NewGuid();
    }

    public static ContextUserManager CreateFromManual(Guid Id)
    {
        return new ContextUserManager(Id);
    }
}