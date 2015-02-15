using System;

namespace OnlineMessenger.Domain.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IMessagesRepository MessageRepository { get; }
        IUsersRepository UserRepository { get; }
        IRolesRepository RoleRepository { get; }
        IUserDataRepository UserDataRepository { get; }
        void Commit();
    }
}