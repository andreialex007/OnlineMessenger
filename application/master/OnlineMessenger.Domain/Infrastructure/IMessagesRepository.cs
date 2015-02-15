using System;
using System.Collections.Generic;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Domain.Infrastructure
{
    public interface IMessagesRepository : IRepository<Message>
    {
        Dictionary<DateTime, int> MessagesPerDay();
    }
}