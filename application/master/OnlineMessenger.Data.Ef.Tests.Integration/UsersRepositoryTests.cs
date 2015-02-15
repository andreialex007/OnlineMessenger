using System;
using System.Collections.Generic;
using NUnit.Framework;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Utils;

// ReSharper disable  InconsistentNaming

namespace OnlineMessenger.Data.Ef.Tests.Integration
{
    public class UsersRepositoryTests : IntegrationTestBase
    {

        #region data
        public List<User> Users = new List<User>
                                  {
                                      new User
                                      {
                                          Email = "Test@mail.ru",
                                          PasswordHash = PasswordHasher.HashPassword("password"),
                                          Name = "Ivanov",
                                          Roles = new List<Role>()
                                      },
                                      new User
                                      {
                                          Email = "Tes111t@mail.ru",
                                          PasswordHash = PasswordHasher.HashPassword("password1"),
                                          Name = "Paul",
                                          Roles = new List<Role>()
                                      },
                                      new User
                                      {
                                          Email = "1111@mail.ru",
                                          PasswordHash = PasswordHasher.HashPassword("password2"),
                                          Name = "Tyrone",
                                          Roles = new List<Role>()
                                      },
                                      new User
                                      {
                                          Email = "5555@mail.ru",
                                          PasswordHash = PasswordHasher.HashPassword("password3"),
                                          Name = "Clara",
                                          Roles = new List<Role>()
                                      },
                                      new User
                                      {
                                          Email = "Ivanov@mail.ru",
                                          PasswordHash = PasswordHasher.HashPassword("password4"),
                                          Name = "April",
                                          Roles = new List<Role>()
                                      },
                                      new User
                                      {
                                          Email = "Petrov@mail.ru",
                                          PasswordHash = PasswordHasher.HashPassword("password5"),
                                          Name = "Kim",
                                          Roles = new List<Role>()
                                      }
                                  };
        #endregion data

        #region test methods

        [Test]
        public void GetUsers_PassSpecificId_ReturnsCorrectCount()
        {
            DbContext.Roles.Add(new Role { Name = "Admin" });
            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        #endregion
    }



}
