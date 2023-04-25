namespace EntityFrameworkHelper.Contracts
{
    public interface ISoftDeletable : IBaseContract
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
