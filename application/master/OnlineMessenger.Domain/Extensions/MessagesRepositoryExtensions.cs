using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.Domain.Extensions
{
    public static class MessagesRepositoryExtensions
    {
        public static IQueryable<Message> GetMessagesByIds(this IMessagesRepository repository, params int[] ids)
        {
            return repository.Query().Where(x => ids.Contains(x.Id));
        }
    }
}
