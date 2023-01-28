namespace EntityFrameworkHelper.Contracts.EntityContracts
{
    public interface IUser : IBaseContract
    {
        public Guid UserId { get; set; }
    }
}
