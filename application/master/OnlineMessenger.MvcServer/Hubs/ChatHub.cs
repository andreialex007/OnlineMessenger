using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using OnlineMessenger.MvcServer.ThreadingTools;

namespace OnlineMessenger.MvcServer.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> ConnectionMapping = new Dictionary<string, string>(); 

        private string UserName
        {
            get { return Context.User != null ? Context.User.Identity.Name : string.Empty; }
        }

        public override Task OnDisconnected()
        {
            var userName = UserName;
            lock (ConnectionMapping)
            {
                if (string.IsNullOrEmpty(UserName))
                    userName = ConnectionMapping[Context.ConnectionId];
            }
            ConnectionTracker.ProcessDisconnection(userName);
            return base.OnDisconnected();
        }

        public override Task OnConnected()
        {
            lock (ConnectionMapping)
            {
                ConnectionMapping.Add(Context.ConnectionId, Context.User.Identity.Name);
            }

            ConnectionTracker.ProcessConnection(UserName);
            return base.OnConnected();
        }
    }
}