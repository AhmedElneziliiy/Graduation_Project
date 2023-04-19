namespace Graduation.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        Task<bool> Complete();
        bool HasChanges(); //use it to check if any changes done to db
    }
}
