using EntityFrameworkHelper.Contracts;

namespace Test.Services;

public class ContextUserManager : IContextUserManager<Guid>
{
    private ContextUserManager(Guid id)
    {
        this.Id = id;
    }
    private Guid Id { get; }
    public Guid GetTenantId()
    {
        return Id != Guid.Empty ? Id : Guid.NewGuid();
    }

    public static ContextUserManager CreateFromManual(Guid id)
    {
        return new ContextUserManager(id);
    }
}