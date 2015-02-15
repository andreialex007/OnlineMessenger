using System;
using OnlineMessenger.Data.Ef.Repositories;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.Data.Ef.Models
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MessengerDbContext _messengerDbContext;

        public UnitOfWork(MessengerDbContext messengerDbContext)
        {
            _messengerDbContext = messengerDbContext;
            MessageRepository = new MessagesRepository(messengerDbContext);
            UserRepository = new UsersRepository(messengerDbContext);
            RoleRepository = new RolesRepository(messengerDbContext);
            UserDataRepository = new UserDataRepository(messengerDbContext);
        }

        public IMessagesRepository MessageRepository { get; private set; }
        public IUsersRepository UserRepository { get; private set; }
        public IRolesRepository RoleRepository { get; private set; }
        public IUserDataRepository UserDataRepository { get; private set; }

        public void Commit()
        {
            _messengerDbContext.SaveChanges();
        }

        public void Dispose()
        {
            _messengerDbContext.Dispose();
        }
    }
}
