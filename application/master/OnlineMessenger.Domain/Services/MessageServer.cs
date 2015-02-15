using System;
using System.Collections.Generic;
using System.Linq;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Exceptions;
using OnlineMessenger.Domain.Extensions;
using OnlineMessenger.Domain.Infrastructure;
using OnlineMessenger.Domain.Utils;

namespace OnlineMessenger.Domain.Services
{
    public class MessageServer : IMessageServer
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBroadcaster _broadcaster;
        private readonly IMailer _mailer;
        private static string LastOperatorNameToWhichUserAssigned = string.Empty;

        public MessageServer(IUnitOfWork unitOfWork, IBroadcaster broadcaster, IMailer mailer)
        {
            _unitOfWork = unitOfWork;
            _broadcaster = broadcaster;
            _mailer = mailer;
        }

        public void SendMail(string userName, string email, string message)
        {
            ValidateMailDataThrowIfInvalid(userName, email, message);
            var fullMessage = string.Format("{0}{1}{2}", message, "<br/>", email);
            _mailer.Send(userName, fullMessage);
        }

        public bool IsAnyOperatorOnline()
        {
            return GetOperators().Any(x => x.IsConnected == true);
        }

        public void AddOperatorToVisible(string clientName, string operatorName)
        {
            var client = _unitOfWork.UserRepository.GetUserByNameWithVisibleUsersAndRoles(clientName);
            var @operator = _unitOfWork.UserRepository.GetUserByName(operatorName);
            client.VisibleToOperators.Add(@operator);
            _unitOfWork.Commit();
        }

        public IEnumerable<Message> GetMessagesOfUsers(string currentUser, string secondUser)
        {
            var second = _unitOfWork.UserRepository.GetUserByNameWithRoles(secondUser);

            if (second.Roles.Count == 0)
                return GetClientMessages(secondUser);

            var current = _unitOfWork.UserRepository.GetUserByName(currentUser);
            var messagesFromFirstUser = _unitOfWork.MessageRepository
                .Query(x => x.From, x => x.To)
                .Where(x => x.From.Id == current.Id && x.To.Any(s => s.Id == second.Id));
            var messagesFromSecondUser = _unitOfWork.MessageRepository.Query(x => x.From, x => x.To)
                .Where(x => x.From.Id == second.Id && x.To.Any(s => s.Id == current.Id));
            var allMessagesBetweenTwo = messagesFromFirstUser.Union(messagesFromSecondUser);
            return allMessagesBetweenTwo.OrderBy(x => x.Date);
        }

        public void DeleteMessages(int[] ids)
        {
            var messages = _unitOfWork.MessageRepository.GetMessagesByIds(ids);
            _unitOfWork.MessageRepository.Delete(messages.ToArray());
            _unitOfWork.Commit();
        }

        public KeyValuePair<int, IEnumerable<Message>> GetMessages(int[] ids = null, string[] fromUsers = null, DateTime? fromDate = null,
            string text = null, string orderBy = null, bool isAsc = false, int? take = null, int? skip = null)
        {
            var total = 0;
            var query = _unitOfWork.MessageRepository.Query(x => x.From, x => x.To);
            if (ids != null && ids.Length > 0)
                query = query.Where(x => ids.Contains(x.Id));
            if (fromDate != null)
                query = query.Where(x => x.Date >= fromDate);
            if (fromUsers != null && fromUsers.Length > 0)
                query = query.Where(x => fromUsers.Select(s => s.ToLower()).Contains(x.From.Name.ToLower()));
            if (!string.IsNullOrEmpty(text))
                query = query.Where(x => x.Text.Contains(text));

            total = query.Count();
            if (orderBy == "From")
                orderBy = "From.Name";

            if (!string.IsNullOrEmpty(orderBy))
                query = query.OrderBy(orderBy, isAsc);


            if (take != null && skip != null)
            {
                query = query.Skip((int)skip)
                    .Take((int)take);
            }
            return new KeyValuePair<int, IEnumerable<Message>>(total, query.ToList());
        }

        public IEnumerable<Message> GetNewMessagesForUser(string userName, int count)
        {
            return _unitOfWork.MessageRepository.Query(x => x.To, x => x.From, x => x.To)
                .Where(x => x.To.Any(u => u.Name == userName))
                .OrderByDescending(x => x.Date)
                .Take(count);
        }

        public void Send(string fromName, string toName, string text)
        {
            var fromUser = _unitOfWork.UserRepository.GetUserByNameWithVisibleUsersAndRoles(fromName);
            var toUser = _unitOfWork.UserRepository.GetUserByNameWithVisibleUsersAndRoles(toName);
            var message = new Message
            {
                From = fromUser,
                Text = text,
                To = new List<User> { toUser }
            };
            _unitOfWork.MessageRepository.Add(message);
            _unitOfWork.Commit();

            var toList = new List<string>();
            toList.Add(toUser.Name);
            toList.AddRange(toUser.VisibleToOperators.Select(x => x.Name));
            _broadcaster.Broadcast(message, toList.ToArray());
        }

        public IEnumerable<User> AllUsers()
        {
            return _unitOfWork.UserRepository.AllUsers();
        }

        public Dictionary<string, IEnumerable<User>> Groups()
        {
            return new Dictionary<string, IEnumerable<User>>
                   {
                       {
                           "Operators",
                           _unitOfWork.UserRepository.Query(x => x.Roles)
                           .Where(x => x.Roles.Any(r => r.Name == Role.OperatorRoleName))
                       },
                       {
                           "Users",
                           _unitOfWork.UserRepository.Query(x => x.Roles)
                           .Where(x => !x.Roles.Any())
                       }
                   };
        }

        public void Send(int fromId, string text, params int[] toIds)
        {
            var fromUser = _unitOfWork.UserRepository.GetUserById(fromId);
            var toUsers = _unitOfWork.UserRepository.GetUsersByIds(toIds);
            var message = new Message
            {
                From = fromUser,
                Text = text,
                To = toUsers.ToList()
            };
            _unitOfWork.MessageRepository.Add(message);
            _unitOfWork.Commit();
            _broadcaster.Broadcast(message, new string[] { });
        }

        public User CreateClient()
        {
            var newUser = NewUser();
            _unitOfWork.UserRepository.Add(newUser);
            _unitOfWork.Commit();
            newUser.Name = string.Format("User{0}", newUser.Id);
            _unitOfWork.Commit();
            return newUser;
        }

        public void AddNewClient(User newUser)
        {
            var operators = GetOperators().Where(x => x.IsConnected == true);
            if (operators.Count() == 0)
                operators = GetOperators();

            var assignedOperator = GetAssignedOperator(operators);
            newUser.Operator = assignedOperator;
            newUser.VisibleToOperators.Add(assignedOperator);
            _unitOfWork.Commit();
        }

        public IEnumerable<User> GetUsersByNames(string[] names)
        {
            return _unitOfWork.UserRepository.GetUsersByNames(names);
        }

        public void SendClientMessage(string userName, string messageText)
        {
            var user = _unitOfWork.UserRepository.Query(x => x.VisibleToOperators, x => x.Operator).SingleOrDefault(x => x.Name == userName);
            var to = user.VisibleToOperators.ToList();
            to.Add(user.Operator);
            var message = new Message
                          {
                              Date = DateTime.Now,
                              From = user,
                              Text = messageText,
                              To = to
                          };
            _unitOfWork.MessageRepository.Add(message);
            if (!user.VisibleToOperators.Select(x => x.Name == user.Operator.Name).Any())
                user.VisibleToOperators.Add(user.Operator);
            user.IsConnected = true;
            _unitOfWork.Commit();
            _broadcaster.Broadcast(message, to.Select(x => x.Name).Distinct().ToArray());
        }

        public void RemoveOperatorFromVisible(string clientName, string operatorName)
        {
            var client = _unitOfWork.UserRepository.GetUserByNameWithVisibleUsersAndRoles(clientName);
            var @operator = _unitOfWork.UserRepository.GetUserByName(operatorName);
            client.VisibleToOperators.Remove(@operator);
            _unitOfWork.Commit();
        }

        public IEnumerable<User> GetVisibleUsersForOperator(string operatorName)
        {
            var user = _unitOfWork.UserRepository.Query(x => x.VisibleClients, x => x.Roles)
                .Single(x => x.Name == operatorName);

            return user.VisibleClients;
        }

        public IEnumerable<User> SearchUsersByName(string name)
        {
            return _unitOfWork.UserRepository.Query(x => x.Roles)
                .Where(
                    x =>
                        x.Roles.All(i => i.Name != Role.OperatorRoleName)
                        && x.Name.ToLower().Contains(name.ToLower())
                );
        }

        public IEnumerable<User> GetOperators()
        {
            return _unitOfWork.UserRepository.GetUsersByRole(Role.OperatorRoleName);
        }

        public IEnumerable<User> GetAdministratorsAndOperators()
        {
            return _unitOfWork.UserRepository.Query(x => x.Roles).Where(
                    x => x.Roles.Any(r => new[] { Role.AdministratorRoleName, Role.OperatorRoleName }.Contains(r.Name)));
        }

        public void SetConnected(string userName, bool connected)
        {
            var user = _unitOfWork.UserRepository.GetUserByName(userName);
            user.IsConnected = connected;
            _unitOfWork.Commit();
        }

        public bool GetConnected(string userName)
        {
            return _unitOfWork.UserRepository.GetUserByName(userName).IsConnected ?? false;
        }

        public void UserDisconnected(string userName)
        {
            var user = _unitOfWork.UserRepository.GetUserByNameWithVisibleUsersAndRoles(userName);
            var usersToNotify = GetUsersToNotify(user);
            _broadcaster.UserDisconnected(user, usersToNotify.ToArray());
        }

        public void UserConnected(string userName)
        {
            var user = _unitOfWork.UserRepository.GetUserByNameWithVisibleUsersAndRoles(userName);
            var usersToNotify = GetUsersToNotify(user);
            _broadcaster.UserConnected(user, usersToNotify.ToArray());
        }

        public void SetDisconnectToAll()
        {
            var connectedUsers = _unitOfWork.UserRepository.Query().Where(x => x.IsConnected == true || x.IsConnected == null);
            foreach (var connectedUser in connectedUsers)
                connectedUser.IsConnected = false;
            _unitOfWork.Commit();
        }

        public IEnumerable<Message> GetClientMessages(string userName)
        {
            var user = _unitOfWork.UserRepository.GetUserByName(userName);
            var userMessages = _unitOfWork.MessageRepository
                .Query(x => x.To, x => x.From)
                .Where(x => x.FromId == user.Id || x.To.Any(r => r.Id == user.Id))
                .OrderBy(x => x.Date).Distinct();
            return userMessages.OrderBy(x => x.Date).ToList();
        }


        #region private methods

        private void ValidateMailDataThrowIfInvalid(string userName, string email, string message)
        {
            if (new[] { email, userName, message }.Any(string.IsNullOrEmpty))
                throw new ValidationException("Empty", "One of fields is empty");
        }

        private bool IsOperator(User user)
        {
            return user.Roles.Any(x => x.Name == Role.OperatorRoleName);
        }

        private bool IsOperatorOrAdministrator(User user)
        {
            return user.Roles.Any(x => x.Name == Role.OperatorRoleName || x.Name == Role.AdministratorRoleName);
        }

        private bool IsClient(User user)
        {
            return !user.Roles.Any();
        }

        private User NewUser()
        {
            return new User
                   {
                       Email = "NewUser@NewUser.com",
                       Name = "NewUser",
                       PasswordHash = PasswordHasher.HashPassword("NewUser")
                   };
        }

        private User GetAssignedOperator(IEnumerable<User> operators)
        {
            var currentOperator = operators.Next(x => x.Name == LastOperatorNameToWhichUserAssigned);
            LastOperatorNameToWhichUserAssigned = currentOperator.Name;
            return currentOperator;
        }

        private List<string> GetUsersToNotify(User connectedUser)
        {
            var usersToNotify = new List<string>();
            if (IsOperatorOrAdministrator(connectedUser))
            {
                usersToNotify.AddRange(
                    GetAdministratorsAndOperators().Select(x => x.Name).Where(x => x != connectedUser.Name));
            }
            else if (IsClient(connectedUser))
            {
                usersToNotify.AddRange(connectedUser.VisibleToOperators.Select(x => x.Name));
            }
            return usersToNotify;
        }

        #endregion private methods

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}