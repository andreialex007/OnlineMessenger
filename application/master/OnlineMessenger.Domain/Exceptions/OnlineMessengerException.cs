using System;

namespace OnlineMessenger.Domain.Exceptions
{
    public class OnlineMessengerException : ApplicationException
    {
        public OnlineMessengerException()
        {
        }

        public OnlineMessengerException(string message)
            : base(message)
        {
        }
    }
}
