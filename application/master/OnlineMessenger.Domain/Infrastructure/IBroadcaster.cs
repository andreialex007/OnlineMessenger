using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Domain.Infrastructure
{
    /// <summary>
    /// Wraps broadcasting api which allow to send server messages to clients
    /// </summary>
    public interface IBroadcaster
    {
        /// <summary>
        /// performs broadcast to several users
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        /// <param name="userNames">UserNames to whom broadcast</param>
        void Broadcast(Message message, string[] userNames);

        /// <summary>
        /// Notifies users about user connection event
        /// </summary>
        /// <param name="user">Connected user</param>
        /// <param name="userNames">Notified user names</param>
        void UserConnected(User user, string[] userNames);

        /// <summary>
        /// Notifies users about user disconnection event
        /// </summary>
        /// <param name="user">Connected user</param>
        /// <param name="userNames">Notified user names</param>
        void UserDisconnected(User user, string[] userNames);
    }
}