namespace EntityFrameworkHelper.Contracts
{
    public interface IUser : IBaseContract
    {
        public Guid UserId { get; set; }
    }
}
