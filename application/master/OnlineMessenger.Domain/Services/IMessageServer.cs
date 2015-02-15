using System;
using System.Collections.Generic;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Domain.Services
{
    public interface IMessageServer : IDisposable
    {
        /// <summary>
        /// send message from user with specific text fromId to users toIds
        /// </summary>
        void Send(int fromId, string text, params int[] toIds);
        void Send(string fromName, string toName, string text);

        /// <summary>
        /// returns users and operators groups
        /// </summary>
        /// <returns></returns>
        Dictionary<string, IEnumerable<User>> Groups();

        /// <summary>
        /// Client new client for chat conversation
        /// </summary>
        User CreateClient();

        /// <summary>
        /// Get conversation messages betweeen two users
        /// </summary>
        IEnumerable<Message> GetMessagesOfUsers(string currentUser, string secondUser);
        IEnumerable<User> GetUsersByNames(string[] names);
        IEnumerable<User> SearchUsersByName(string name);
        IEnumerable<User> GetOperators();

        /// <summary>
        /// get all client messsages from user
        /// </summary>
        IEnumerable<Message> GetClientMessages(string userName);
        IEnumerable<User> GetVisibleUsersForOperator(string operatorName);
        IEnumerable<User> GetAdministratorsAndOperators();
        IEnumerable<Message> GetNewMessagesForUser(string userName, int count);
        KeyValuePair<int, IEnumerable<Message>> GetMessages(int[] ids = null, string[] fromUsers = null,
            DateTime? fromDate = null,
            string text = null, string orderBy = null, bool isAsc = false, int? take = null, int? skip = null);

        
        void AddNewClient(User newUser);
        void UserDisconnected(string userName);
        void UserConnected(string userName);
        void SendClientMessage(string userName, string messageText);
        void SetConnected(string userName, bool connected);
        bool GetConnected(string userName);
        void SetDisconnectToAll();
        void AddOperatorToVisible(string clientName, string operatorName);
        void RemoveOperatorFromVisible(string clientName, string operatorName);
        void DeleteMessages(int[] ids);
        void SendMail(string userName, string email, string message);
        bool IsAnyOperatorOnline();
    }
}