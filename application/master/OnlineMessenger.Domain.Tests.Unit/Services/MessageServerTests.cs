using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Moq;
using NUnit.Framework;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Exceptions;
using OnlineMessenger.Domain.Infrastructure;
using OnlineMessenger.Domain.Services;
// ReSharper disable  InconsistentNaming

namespace OnlineMessenger.Domain.Tests.Unit.Services
{
    public class MessageServerTests : TestsBase
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMailer> _mailer;
        private Mock<IBroadcaster> _broadcaster;
        private Mock<IUsersRepository> _userRepository;
        private Mock<IMessagesRepository> _messageRepository;
        private IMessageServer _messageServer;

        #region setup/teardown

        [SetUp]
        public void SetUp()
        {
            InitMocks();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion setup/teardown

        #region common data

        private static Role OperatorRole = new Role { Id = 1, Name = Role.OperatorRoleName };

        private static User Client = new User
                              {
                                  Id = 2,
                                  Name = "User",
                                  Roles = new List<Role>(),
                                  Email = "Email111@mail.ru"
                              };

        private static User OperatorUser = new User
                                    {
                                        Id = 1,
                                        Name = "Operator",
                                        Roles = new List<Role> { OperatorRole },
                                        Email = "Email@mail.ru",
                                        VisibleToOperators = new List<User>()
                                    };
        private static User AnotherOperator = new User
                                            {
                                                Id = 3,
                                                Name = "Operator2",
                                                Roles = new List<Role> { OperatorRole },
                                                Email = "Email1112121@mail.ru",
                                                VisibleToOperators = new List<User>()
                                            };

        private List<Message> Messages = new List<Message>
                                         {
                                             new Message
                                             {
                                                 Date = DateTime.Now.AddDays(1),
                                                 From = Client,
                                                 FromId = Client.Id,
                                                 Id = 5,
                                                 Text = "text",
                                                 To = new List<User> {OperatorUser}
                                             },
                                             new Message
                                             {
                                                 Date = DateTime.Now.AddDays(-1),
                                                 From = OperatorUser,
                                                 Id = 6,
                                                 FromId = OperatorUser.Id,
                                                 Text = "text2",
                                                 To = new List<User> {Client}
                                             },
                                             new Message
                                             {
                                                 Date = DateTime.Now.AddDays(-5),
                                                 From = AnotherOperator,
                                                 Id = 7,
                                                 FromId = AnotherOperator.Id,
                                                 Text = "text233",
                                                 To = new List<User> {OperatorUser}
                                             }
                                         };

        #endregion common data


        #region testmethods

        [Test]
        public void Send_ValidParametersPassed_MessageSended()
        {
            //arrange
            var fromId = 2;
            var fromUser = new User { Id = 2, Name = "Petrenko", Email = "petr@mail.ru" };
            var toIds = new[] { 5, 10, 20 };
            var text = "test text";
            var toUsers = new[]
                          {
                              new User {Id = 5, Name = "Ivanov", Email = "Ivanov@mail.ru"}, new User {Id = 10, Name = "Petrov", Email = "Petrov@mail.ru"},
                              new User {Id = 20, Name = "Sidorov", Email = "Sidorov@mail.ru"}
                          };
            var allUsers = new List<User>();
            allUsers.AddRange(toUsers);
            allUsers.Add(fromUser);
            _userRepository.Setup(x => x.Query()).Returns(allUsers.AsQueryable());
            //_userRepository.Setup(x => x.Query()).Returns(new List<User> { fromUser }.AsQueryable());

            //act
            _messageServer.Send(fromId, text, toIds);

            //assert
            _broadcaster.Verify(x => x.Broadcast(
                It.Is<Message>(v => v.To.SequenceEqual(toUsers) && v.From == fromUser && v.Text == text), new string[] { }));
        }

        [Test]
        public void Send_ValidParametersPassed_MessageAdded()
        {
            //arrange
            var fromId = 2;
            var fromUser = new User { Id = 2, Name = "Petrenko", Email = "petr@mail.ru" };
            var toIds = new[] { 5, 10, 20 };
            var text = "test text";
            var toUsers = new[]
                          {
                              new User {Id = 5, Name = "Ivanov", Email = "Ivanov@mail.ru"}, new User {Id = 10, Name = "Petrov", Email = "Petrov@mail.ru"},
                              new User {Id = 20, Name = "Sidorov", Email = "Sidorov@mail.ru"}
                          };
            var allUsers = new List<User>();
            allUsers.AddRange(toUsers);
            allUsers.Add(fromUser);
            _userRepository.Setup(x => x.Query()).Returns(allUsers.AsQueryable());

            //act
            _messageServer.Send(fromId, text, toIds);

            //assert
            _messageRepository.Verify(x =>
                x.Add(It.Is<Message>(
                    v => v.To.SequenceEqual(toUsers)
                    && v.From == fromUser
                    && v.Text == text)));
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void SendMail_PassInvalidParameters_ThrowValidationException()
        {
            _messageServer.SendMail(string.Empty, string.Empty, string.Empty);
        }

        [Test]
        public void SendMail_PassCorrectParameters_EmailMessageSended()
        {
            var userName = "Ivanov";
            var email = "Email@email.ru";
            var message = "message";

            //act
            _messageServer.SendMail(userName, email, message);

            //assert
            _mailer.Verify(x => x.Send(userName, string.Format("{0}{1}{2}", message, "<br/>", email)));
        }

        [Test]
        public void AddOperatorToVisible_ClientNameOperatorNamePassed_OperatorAdded()
        {
            //arrange
            var operatorRole = new Role { Id = 1, Name = Role.OperatorRoleName };
            var operatorUser = new User
                               {
                                   Id = 1,
                                   Name = "Operator",
                                   Roles = new List<Role> { operatorRole },
                                   Email = "Email@mail.ru",
                                   VisibleToOperators = new List<User>()
                               };
            var client = new User
                           {
                               Id = 2,
                               Name = "User",
                               Roles = new List<Role>(),
                               Email = "Email111@mail.ru"
                           };
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(new EnumerableQuery<User>(new List<User> { operatorUser, client }));

            //act
            _messageServer.AddOperatorToVisible(client.Name, operatorUser.Name);

            //assert
            Assert.IsTrue(Object.Equals(client.VisibleToOperators.First(), operatorUser));
            _unitOfWork.Verify(x => x.Commit());

        }

        [Test]
        public void GetClientMessages_PassedUserName_ReturnsCorrectList()
        {
            //arrange
            _messageRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Message, object>>[]>()))
                 .Returns(() => new EnumerableQuery<Message>(Messages));
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(new EnumerableQuery<User>(new List<User> { Client }));

            //act
            var clientMessages = _messageServer.GetClientMessages(Client.Name);

            //assert
            Assert.IsTrue(Object.Equals(Messages.First(), clientMessages.Last()));
            Assert.IsTrue(Object.Equals(Messages[1], clientMessages.First()));
        }


        [Test]
        public void GetMessagesOfUsers_IfSecondUserCLient_ReturnClientMessages()
        {
            //arrange
            _messageRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Message, object>>[]>()))
                 .Returns(() => new EnumerableQuery<Message>(Messages));
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(new EnumerableQuery<User>(new List<User> { Client }));

            //act
            var returnedMessages = _messageServer.GetMessagesOfUsers(OperatorUser.Name, Client.Name);

            //assert
            Assert.IsTrue(Object.Equals(Messages.First(), returnedMessages.Last()));
            Assert.IsTrue(Object.Equals(Messages[1], returnedMessages.First()));
        }

        [Test]
        public void GetMessagesOfUsers_IfBothUsersOperators_ReturnCorrectList()
        {
            //arrange
            _messageRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Message, object>>[]>()))
                 .Returns(() => new EnumerableQuery<Message>(Messages));
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(new EnumerableQuery<User>(new List<User> { Client, OperatorUser, AnotherOperator }));

            //act
            var returnedMessages = _messageServer.GetMessagesOfUsers(OperatorUser.Name, AnotherOperator.Name);

            //assert
            Assert.IsTrue(Object.Equals(Messages[2], returnedMessages.First()));
        }

        [Test]
        public void GetMessages_PerformComplexQuery_ReturnCorrectList()
        {
            //arrange
            _messageRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Message, object>>[]>()))
                 .Returns(() => new EnumerableQuery<Message>(Messages));

            var gottedMEssages = _messageServer.GetMessages(new int[] { 5, 6, 7 },
                new string[] { Client.Name, OperatorUser.Name, AnotherOperator.Name }, DateTime.Now.AddDays(-50), "text",
                "From", true, 1, 0);

            Assert.IsTrue(Object.Equals(Messages[1], gottedMEssages.Value.First()));
            Assert.AreEqual(Messages.Count, gottedMEssages.Key);

        }


        [Test]
        public void GetNewMessagesForUser_PassedUserNameAndCount_ReturnCorrectList()
        {
            var secondMessage = new Message
                                {
                                    Date = DateTime.Now.AddDays(-5),
                                    From = OperatorUser,
                                    Id = 7,
                                    FromId = OperatorUser.Id,
                                    Text = "text233",
                                    To = new List<User> { OperatorUser }
                                };
            var inputMessages = new[]
                                {
                                    new Message
                                    {
                                        Date = DateTime.Now.AddDays(-1),
                                        From = OperatorUser,
                                        Id = 6,
                                        FromId = OperatorUser.Id,
                                        Text = "text2",
                                        To = new List<User> {Client}
                                    },
                                    secondMessage
                                };

            //arrange
            _messageRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Message, object>>[]>()))
                 .Returns(() => new EnumerableQuery<Message>(inputMessages));

            //act
            var outPutMessages = _messageServer.GetNewMessagesForUser(OperatorUser.Name, 1);

            //asesert
            Assert.IsTrue(Object.Equals(outPutMessages.ElementAt(0), outPutMessages.ElementAt(0)));
        }


        [Test]
        public void Send_PassedNamesAndText_MessageAddedToRepositoryAndBroadcasted()
        {
            //arrange
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new List<User> { Client, OperatorUser, AnotherOperator }.AsQueryable());
            _messageRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Message, object>>[]>()))
                .Returns(() => new List<Message>(Messages).AsQueryable());
            var text = "Hi!";

            //act
            _messageServer.Send(OperatorUser.Name, AnotherOperator.Name, text);

            //assert
            _messageRepository.Verify(
                x => x.Add(It.Is<Message>(t => t.Text == text && t.From.Name == OperatorUser.Name &&
                                              t.To.Any(a => a.Name == AnotherOperator.Name))));

        }

        [Test]
        public void Groups_Called_ReturnsCorrectGroups()
        {
            //arrange
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new List<User> { Client, OperatorUser, AnotherOperator }.AsQueryable());

            //act
            var groups = _messageServer.Groups();

            //assert
            Assert.IsTrue(Object.Equals(groups.ElementAt(0).Value.ElementAt(0), OperatorUser));
            Assert.IsTrue(Object.Equals(groups.ElementAt(0).Value.ElementAt(1), AnotherOperator));
            Assert.IsTrue(Object.Equals(groups.ElementAt(1).Value.First(), Client));

        }

        [Test]
        public void CreateChatUser_Called_CorrectUserReturned()
        {
            //arrange
            int expectedId = 20;
            _userRepository.Setup(x => x.Add(It.IsAny<User>())).Callback<User>(m => m.Id = expectedId);

            //act
            var user = _messageServer.CreateClient();

            //assert
            Assert.IsTrue(user.Name == string.Format("User{0}", expectedId));
            Assert.IsTrue(user.Id == 20);
            _unitOfWork.Verify(x => x.Commit());
        }

        [Test]
        public void AddNewClient_NewUserClientAdded_UserAssignedToOperatorAndAddedToVisibleUsers()
        {
            //arrange
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new List<User> { Client, OperatorUser, AnotherOperator }.AsQueryable());
            var newClient = new User { Id = 2, Name = "Petrenko", Email = "petr@mail.ru" }; ;

            //act
            _messageServer.AddNewClient(newClient);

            //assert
            Assert.IsTrue(Object.Equals(newClient.VisibleToOperators.First(), OperatorUser));
            Assert.IsTrue(newClient.Operator == OperatorUser);
        }

        [Test]
        public void SendClientMessage_PassedNameAndText_MessageAddedToRepoAndBroadcasted()
        {
            //arrange
            Client.VisibleToOperators.AddRange(new User[] { OperatorUser, AnotherOperator });
            Client.Operator = OperatorUser;
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new List<User> { Client, OperatorUser, AnotherOperator }.AsQueryable());
            var expectedText = "Hi!";

            //act
            _messageServer.SendClientMessage(Client.Name, expectedText);

            //assert
            _messageRepository.Setup(
                x => x.Add(It.Is<Message>(message => message.Text == expectedText && message.From.Name == Client.Name)));
            _broadcaster.Verify(
                x => x.Broadcast(It.Is<Message>(m => m.Text == expectedText), It.IsAny<string[]>()));

        }

        [Test]
        public void RemoveOperatorFromVisible_ClientNameOperatorNamePassed_OperatorRemoved()
        {
            //arrange
            Client.VisibleToOperators.AddRange(new User[] { OperatorUser });
            Client.Operator = OperatorUser;
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new List<User> { Client, OperatorUser, AnotherOperator }.AsQueryable());

            //act
            _messageServer.RemoveOperatorFromVisible(Client.Name, OperatorUser.Name);

            Assert.IsTrue(!Client.VisibleToOperators.Any());
            _unitOfWork.Verify(x => x.Commit());
        }

        [Test]
        public void SearchUsersByName_UserNamePassed_ReturnsCorrectUsers()
        {
            //arrange
            Client.VisibleToOperators.AddRange(new User[] { OperatorUser });
            Client.Operator = OperatorUser;
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new List<User> { Client, OperatorUser, AnotherOperator }.AsQueryable());

            //act
            var users = _messageServer.SearchUsersByName("Use");

            Assert.IsTrue(users.Count() == 1);
            Assert.IsTrue(Object.Equals(users.First(), Client));
        }

        [Test]
        public void UserDisconnected_UserDisconnected_OperatorsToWhomUserIsVisibleNotified()
        {
            //arrange
            Client.VisibleToOperators.AddRange(new User[] { OperatorUser, AnotherOperator });
            Client.Operator = OperatorUser;
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new List<User> { Client, OperatorUser, AnotherOperator }.AsQueryable());
            var expectedToNotify = new string[] { OperatorUser.Name, AnotherOperator.Name };

            //act
            _messageServer.UserDisconnected(Client.Name);

            //assert
            _broadcaster.Verify(
                x => x.UserDisconnected(It.Is<User>(u => u.Equals(Client)),
                    It.Is<string[]>(strings => !strings.Except(expectedToNotify).Any())));

        }

        [Test]
        public void UserDisconnected_OperatorDisconnected_OperatorsNotified()
        {
            //arrange
            Client.VisibleToOperators.AddRange(new User[] { OperatorUser, AnotherOperator });
            Client.Operator = OperatorUser;
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new List<User> { Client, OperatorUser, AnotherOperator }.AsQueryable());
            var expectedToNotify = new string[] { AnotherOperator.Name };

            //act
            _messageServer.UserDisconnected(OperatorUser.Name);

            //assert
            _broadcaster.Verify(
                x => x.UserDisconnected(It.Is<User>(u => u.Equals(OperatorUser)),
                    It.Is<string[]>(strings => !strings.Except(expectedToNotify).Any())));

        }

        [Test]
        public void UserConnected_OperatorConnected_OperatorsNotified()
        {
            //arrange
            Client.VisibleToOperators.AddRange(new User[] { OperatorUser, AnotherOperator });
            Client.Operator = OperatorUser;
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new List<User> { Client, OperatorUser, AnotherOperator }.AsQueryable());
            var expectedToNotify = new string[] { AnotherOperator.Name };


            //act
            _messageServer.UserConnected(OperatorUser.Name);

            //assert
            _broadcaster.Verify(
                x => x.UserConnected(It.Is<User>(u => u.Equals(OperatorUser)),
                    It.Is<string[]>(strings => !strings.Except(expectedToNotify).Any())));

        }

        [Test]
        public void SetDisconnectToAll_AllUsersDisconnected()
        {
            var allUsers = new List<User> { Client, OperatorUser, AnotherOperator };
            allUsers.ForEach(user => { user.IsConnected = true; });
            _userRepository.Setup(x => x.Query())
                .Returns(allUsers.ToList().AsQueryable());

            _messageServer.SetDisconnectToAll();

            Assert.IsTrue(allUsers.TrueForAll(x => x.IsConnected == false));
        }




        private void InitMocks()
        {
            _userRepository = new Mock<IUsersRepository>();
            _broadcaster = new Mock<IBroadcaster>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _messageRepository = new Mock<IMessagesRepository>();
            _unitOfWork.SetupGet(x => x.UserRepository).Returns(_userRepository.Object);
            _unitOfWork.SetupGet(x => x.MessageRepository).Returns(_messageRepository.Object);
            _mailer = new Mock<IMailer>();
            _messageServer = new MessageServer(_unitOfWork.Object, _broadcaster.Object, _mailer.Object);
        }

        #endregion testmethods

    }





}
