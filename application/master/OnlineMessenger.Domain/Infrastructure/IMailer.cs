using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMessenger.Domain.Infrastructure
{
    public interface IMailer
    {
        void Send(string from, string text);
    }
}
