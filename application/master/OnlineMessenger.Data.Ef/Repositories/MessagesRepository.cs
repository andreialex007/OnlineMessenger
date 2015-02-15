using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.Data.Ef.Repositories
{
    public class MessagesRepository : RepositoryBase<Message>, IMessagesRepository
    {
        public MessagesRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Dictionary<DateTime, int> MessagesPerDay()
        {
            return Query()
                .GroupBy(x => DbFunctions.TruncateTime(x.Date), x => x)
                .ToDictionary(x => x.Key.Value, x => x.Count());
        }
    }
}
