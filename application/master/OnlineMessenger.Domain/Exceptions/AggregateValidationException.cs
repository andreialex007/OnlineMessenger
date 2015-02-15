using System.Collections.Generic;

namespace OnlineMessenger.Domain.Exceptions
{
    public class AggregateValidationException<T> : OnlineMessengerException where T : new()
    {
        public AggregateValidationException(List<EntityErrors<T>> entityErrors)
        {
            EntityErrors = entityErrors;
        }

        public List<EntityErrors<T>> EntityErrors { get; set; }

    }
}
