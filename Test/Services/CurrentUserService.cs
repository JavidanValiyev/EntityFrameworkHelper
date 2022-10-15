using EntityFrameworkHelper.Contracts;

namespace Test.Services;

public class CurrentUserService : ICurrentUserService<Guid>
{
    private CurrentUserService(Guid Id)
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

    public static CurrentUserService CreateFromManual(Guid Id)
    {
        return new CurrentUserService(Id);
    }
}