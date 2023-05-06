namespace EntityFrameworkHelper.Contracts
{
    internal interface IUser : IBaseContract
    {
        public Guid UserId { get; set; }
    }
}
