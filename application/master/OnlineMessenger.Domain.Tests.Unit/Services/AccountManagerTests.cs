using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Exceptions;
using OnlineMessenger.Domain.Infrastructure;
using OnlineMessenger.Domain.Services;
using OnlineMessenger.Domain.Utils;

// ReSharper disable  InconsistentNaming

namespace OnlineMessenger.Domain.Tests.Unit.Services
{
    public class AccountManagerTests : TestsBase
    {
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

        #endregion

        private const string StringMore250SignsLength = @"LongLongStringLongLong
            StringLongLongStringLongLongStringLongLongStringLongLongStringLongLongStringLongLongString
            LongLongStringLongLongStringLongLongStringLongLongStringLongLongStringLongLong
            StringLongLongStringLongLongStringLongLongStringLongLongString";
        private Mock<IUsersRepository> _userRepository;
        private Mock<IUserDataRepository> _userDataRepository;
        private Mock<IRolesRepository> _roleRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private IAccountManger _accountManger;

        #region helper methods

        private void InitMocks()
        {
            _userRepository = new Mock<IUsersRepository>();
            _roleRepository = new Mock<IRolesRepository>();
            _userDataRepository = new Mock<IUserDataRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.SetupGet(x => x.UserRepository).Returns(_userRepository.Object);
            _unitOfWork.SetupGet(x => x.RoleRepository).Returns(_roleRepository.Object);
            _unitOfWork.SetupGet(x => x.UserDataRepository).Returns(_userDataRepository.Object);
            _accountManger = new AccountManager(_unitOfWork.Object);
        }

        #endregion

        #region test methods

        [Test]
        public void CreateUser_AcceptUserAndPassword_CreateValidUser()
        {
            //arrange
            int userId = 10;
            _userRepository.Setup(x => x.Add(It.IsAny<User>())).Callback<User>(user1 => { user1.Id = userId; });
            string password = "password";
            var user = new User { Name = "Ivanov", Email = "user@mai.ru" };

            //act
            _accountManger.CreateUser(user, password);

            //assert
            _userRepository.Verify(
                repository => repository.Add(It.Is<User>(u =>
                    !string.IsNullOrEmpty((u).PasswordHash)
                    && u.Email == user.Email
                    && u.Name == user.Name
                    )));
            Assert.AreEqual(userId, user.Id);
        }

        [Test]
        public void CreateUser_AcceptUnvalidUser_ThrowsValidationException()
        {
            //arrange
            string password = "password";
            var user = new User { Name = "", Email = "invalid email" };

            try
            {
                _accountManger.CreateUser(user, password);
                Assert.Fail();
            }
            catch (ValidationException exception)
            {
                Assert.IsTrue(exception.ValidationErrors.Select(x => x.Name).ContainsAll(new[] { "Name", "Email" }));
            }
        }

        [Test]
        public void LogIn_PassedNotExistedUserName_ReturnsEmptyUser()
        {
            //arrange
            string validPassword = "#1GHN4st256i";
            string validPasswordHash = PasswordHasher.HashPassword(validPassword);
            var expectedUser = new User { Name = "ivanov", PasswordHash = validPasswordHash };
            _userRepository.Setup(x => x.Query()).Returns(new List<User> { expectedUser }.AsQueryable());

            //act
            var user = _accountManger.CheckPassword("some username", "");

            //assert
            Assert.IsNull(user);
        }

        [Test]
        public void LogIn_PassedValidAndCorrectPasswordAndUserName_ReturnsUser()
        {
            //arrange
            string validPassword = "#1GHN4st256i";
            string validPasswordHash = PasswordHasher.HashPassword(validPassword);
            var expectedUser = new User { Name = "ivanov", PasswordHash = validPasswordHash };
            _userRepository.Setup(x => x.Query(r => r.Roles)).Returns(new List<User> { expectedUser }.AsQueryable());

            //act
            User user = _accountManger.CheckPassword("ivanov", validPassword);

            //assert
            Assert.AreEqual(expectedUser.Name, user.Name);
        }

        [Test]
        public void SetPassword_NewPasswordAndUserNamePassed_UserHashStored()
        {
            //arrange
            string password = "#1GHN4st256i";
            string passwordHashedExpected = PasswordHasher.HashPassword(password);
            var user = new User { Name = "Ivanov" };
            _userRepository.Setup(x => x.Query()).Returns(new List<User> { user }.AsQueryable());


            //act
            _accountManger.SetPassword(user.Name, password);

            //assert
            Assert.AreEqual(passwordHashedExpected, user.PasswordHash);
        }

        [Test]
        public void SetPassword_WrongPasswordPassed_ErrorsReturned(
            [Values("12", "", StringMore250SignsLength)] string password)
        {
            //arrange
            var user = new User { Name = "Ivanov" };


            //assert
            try
            {
                _accountManger.SetPassword(user.Name, password);
                Assert.Fail();
            }
            catch (ValidationException exception)
            {
                Assert.IsTrue(exception.ValidationErrors.Select(x => x.Name).ContainsAll(new[] { "Password" }));
            }
        }

        [Test]
        public void UpdateUser_PassedInvalidUser_ErrorsAdded([Values("", "11")] string userName,
            [Values("", "111111")] string email)
        {
            //arrange
            int userId = 10;
            _userRepository.Setup(x => x.Update(It.IsAny<User>())).Callback<User>(user1 => { user1.Id = userId; });
            var user = new User { Name = userName, Email = email };

            try
            {
                _accountManger.UpdateUserSettings(user);
                Assert.Fail();
            }
            catch (ValidationException exception)
            {
                Assert.IsTrue(exception.ValidationErrors.Select(x => x.Name).ContainsAll(new[] { "Name", "Email" }));
            }
        }

        [Test]
        public void UpdateUserSettings_PassedValidUser_SavingSucceeded()
        {
            //arrange
            var oldUser = new User
            {
                Name = "UserName",
                Email = "OldUser@mai.ru",
                AudioNotificationsEnabled = false,
                VisualNotificationsEnabled = false
            };

            var newUser = new User
            {
                Name = "UserName",
                Email = "NewUser@mai.ru",
                AudioNotificationsEnabled = true,
                VisualNotificationsEnabled = true
            };

            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>())).Returns(new List<User> { oldUser }.AsQueryable());

            //act
            _accountManger.UpdateUserSettings(newUser);


            //assert
            Assert.AreEqual(oldUser.Email, newUser.Email);
            Assert.AreEqual(oldUser.AudioNotificationsEnabled, newUser.AudioNotificationsEnabled);
            Assert.AreEqual(oldUser.VisualNotificationsEnabled, newUser.VisualNotificationsEnabled);
        }

        [Test]
        [ExpectedException(typeof(AggregateValidationException<Role>))]
        public void UpdateOrCreate_RoleWithEmptyNamePassed_throwsValidationException()
        {
            var role = new Role { Name = string.Empty };
            _roleRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Role, object>>[]>()))
                .Returns(() => new EnumerableQuery<Role>(new List<Role> { new Role() }));

            _accountManger.UpdateOrCreate(new[] { role });
        }

        [Test]
        public void UpdateOrCreate_IfPassedNewRole_AddedToRepository()
        {
            //arrange
            var role = new Role { Name = "Test role" };
            _roleRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Role, object>>[]>()))
                .Returns(() => new EnumerableQuery<Role>(new List<Role> { new Role() }));

            //act
            _accountManger.UpdateOrCreate(new Role[] { role });

            //assert
            _roleRepository.Verify(x => x.Add(It.Is<Role>(t => t.Name == role.Name && t.Id == 0)));
            _unitOfWork.Verify(x => x.Commit());
        }


        [Test]
        public void UpdateOrCreate_IfPassedChangedRole_NameChanged()
        {
            //arrange
            var role = new Role { Id = 10, Name = "New role name" };
            var oldRole = new Role { Id = 10, Name = "Old role name" };

            _roleRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Role, object>>[]>()))
                .Returns(() => new EnumerableQuery<Role>(new List<Role> { oldRole }));

            //act
            _accountManger.UpdateOrCreate(new[] { role });

            //assert
            Assert.True(oldRole.Name == role.Name);
            _unitOfWork.Verify(x => x.Commit());
        }

        [Test]
        [ExpectedException(typeof(AggregateValidationException<User>))]
        public void UpdateOrCreate_UserNameWithEmptyNamePassed_throwsValidationException()
        {
            var passwordPair = new UserPasswordPair(new User { }, false, "new password");
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new EnumerableQuery<User>(new List<User> { new User() }));

            _accountManger.UpdateOrCreate(new[] { passwordPair });
        }

        [Test]
        public void UpdateOrCreate_IfPassedNewUser_AddedToRepository()
        {
            //arrange
            var passwordPair = new UserPasswordPair(new User { Name = "Ivanov", Email = "ljsfljl@jljfkljk.ru" }, false, "new password");
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new EnumerableQuery<User>(new List<User> { new User() }));

            //act
            _accountManger.UpdateOrCreate(new[] { passwordPair });

            //assert
            _userRepository.Verify(x => x.Add(It.Is<User>(t => t.Name == passwordPair.User.Name && t.Id == 0)));
        }


        [Test]
        public void UpdateOrCreate_IfPassedUserNameChanged_NameChanged()
        {
            //arrange
            var newUser = new User { Id = 10, Name = "NewIvanov", Email = "NewEmail@jljfkljk.ru" };
            var oldUser = new User { Id = 10, Name = "OldIvanov", Email = "OldEmail@jljfkljk.ru" };
            var passwordPair = new UserPasswordPair(newUser, false, "new password");
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new EnumerableQuery<User>(new List<User> { oldUser }));

            //act
            _accountManger.UpdateOrCreate(new[] { passwordPair });

            //assert
            Assert.True(newUser.Name == oldUser.Name);
            Assert.True(newUser.Email == oldUser.Email);
            _unitOfWork.Verify(x => x.Commit());
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ChangePassword_ProvidedWrongPassword_ThrowsValidtionException()
        {
            //arrange
            const string currentPassword = "password123";
            var hashedPassword = PasswordHasher.HashPassword(currentPassword);
            var ivanov = new User
                         {
                             Name = "Ivanov",
                             Email = "Ivanov@mail.ru",
                             PasswordHash = hashedPassword
                         };
            var wrongPassword = "wrong password";
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new EnumerableQuery<User>(new List<User> { ivanov }));

            //act
            _accountManger.ChangePassword(ivanov.Name, wrongPassword, It.IsAny<string>(), It.IsAny<string>());
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ChangePassword_PasswordNotEqualPasswordConfirm_ThrowsValidationException()
        {
            //arrange
            const string currentPassword = "password123";
            var hashedPassword = PasswordHasher.HashPassword(currentPassword);
            var ivanov = new User
            {
                Name = "Ivanov",
                Email = "Ivanov@mail.ru",
                PasswordHash = hashedPassword
            };
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new EnumerableQuery<User>(new List<User> { ivanov }));

            //act
            _accountManger.ChangePassword(ivanov.Name, currentPassword, "password", "passwordConfirm");
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void SetPassword_PassedNotStrongPassword_ThrowValidationException()
        {
            var hashedPassword = PasswordHasher.HashPassword("password123");
            var ivanov = new User
            {
                Name = "Ivanov",
                Email = "Ivanov@mail.ru",
                PasswordHash = hashedPassword
            };

            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new EnumerableQuery<User>(new List<User> { ivanov }));

            _accountManger.SetPassword(ivanov.Name, string.Empty);
        }

        [Test]
        public void GetUsers_FindUserByComplexQuery_ReturnCorrectUsers()
        {
            var users = new[]
                        {
                            new User
                            {
                                Id = 1,
                                Name = "Ivanov",
                                CreatedDate = DateTime.Now.AddDays(-20),
                                Email = "Ivanov@Ivanov.ru",
                                Roles = new List<Role> {new Role {Id = 1, Name = "Role1"}}
                            },
                            new User
                            {
                                Id = 2,
                                Name = "Petrov",
                                CreatedDate = DateTime.Now.AddDays(-5),
                                Email = "Petrov@Ivanov.ru",
                            },
                            new User
                            {
                                Id = 3,
                                Name = "Sidorov",
                                CreatedDate = DateTime.Now.AddDays(3),
                                Email = "Sidorov@Ivanov.ru",
                                Roles = new List<Role> {new Role {Id = 2, Name = "Role2"}}
                            },
                            new User
                            {
                                Id = 4,
                                Name = "Kovalev",
                                CreatedDate = DateTime.Now.AddDays(30),
                                Email = "Kovalev@Ivanov.ru",
                            },
                        }.ToList();

            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new EnumerableQuery<User>(users));

            var foundUsers = _accountManger.GetUsers(new int[] { 1, 2, 3 }, new string[] { "Ivanov", "Petrov", "Kovalev" },
                DateTime.Now.AddDays(-30), new string[] { "Ivanov@Ivanov.ru", "Petrov@Ivanov.ru" },
                new string[] { "Role1", "Role2" }, "Roles.Count", true);

            Assert.True(foundUsers.Count() == 1);
            Assert.True(Object.Equals(foundUsers.First(), users.First()));
        }

        [Test]
        public void GetRoles_FindUserByComplexQuery_ReturnCorrectRoles()
        {
            //ararnge
            var users = new[]
                        {
                            new User
                            {
                                Id = 3,
                                Name = "Sidorov",
                                CreatedDate = DateTime.Now.AddDays(3),
                                Email = "Sidorov@Ivanov.ru",
                                Roles = new List<Role> {new Role {Id = 2, Name = "Role2"}}
                            },
                            new User
                            {
                                Id = 4,
                                Name = "Kovalev",
                                CreatedDate = DateTime.Now.AddDays(30),
                                Email = "Kovalev@Ivanov.ru",
                            }
                        }.ToList();

            var roles = new[]
                        {
                            new Role {Id = 1, Name = "Role1", Users = users},
                            new Role {Id = 2, Name = "Role2", Users = new List<User>()},
                            new Role {Id = 3, Name = "Role3", Users = new List<User>()}
                        };

            _roleRepository.Setup(x => x.Query(It.IsAny<Expression<Func<Role, object>>[]>()))
                .Returns(() => new EnumerableQuery<Role>(roles.ToArray()));

            //act
            var foundRoles = _accountManger.GetRoles(new[] { 1, 2, 3 }, new[] { "Role1", "Role2" },
                new[] { "Sidorov", "Kovalev" }, "UsersCount", true);

            //assert
            Assert.True(foundRoles.Count() == 1);
            Assert.True(Object.Equals(foundRoles.First(), roles.First()));
        }

        [Test]
        public void UploadUserAvatar_UserNameAndBytesPassed_UserDataAdded()
        {
            //arrange
            var ivanov = new User
            {
                Name = "Ivanov",
                Email = "Ivanov@mail.ru",
                PasswordHash = PasswordHasher.HashPassword("123456")
            };
            _userRepository.Setup(x => x.Query(It.IsAny<Expression<Func<User, object>>[]>()))
                .Returns(() => new EnumerableQuery<User>(new List<User> { ivanov }));

            var image = Image.FromHbitmap(new Bitmap(100, 100).GetHbitmap());
            var converter = new ImageConverter();
            var bytes = (byte[])converter.ConvertTo(image, typeof(byte[]));

            //act
            _accountManger.UploadUserAvatar("Ivanov", bytes);

            //assert
            _userDataRepository.Verify(x => x.Add(It.IsAny<UserData>()));
        }

        #endregion
    }

}