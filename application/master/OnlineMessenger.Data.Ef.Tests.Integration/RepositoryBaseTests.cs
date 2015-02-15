using System;
using NUnit.Framework;
using OnlineMessenger.Data.Ef.Repositories;
using OnlineMessenger.Domain.Entities;

// ReSharper disable InconsistentNaming

namespace OnlineMessenger.Data.Ef.Tests.Integration
{
    public class RepositoryBaseTests : IntegrationTestBase
    {

        private RepositoryBase<User> RepositoryBase;

        public override void SetUp()
        {
            base.SetUp();
            RepositoryBase = new RepositoryBase<User>(DbContext);
        }

        [Test]
        public void Update_EntityPassed_UpdateSucceeded()
        {
            //arrange
            var added = new User { Name = "Admin", Email = "Test@Email.ru", PasswordHash = "hash" };
            DbContext.Users.Add(added);
            DbContext.SaveChanges();

            //act
            try
            {
                RepositoryBase.Update(added);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}
