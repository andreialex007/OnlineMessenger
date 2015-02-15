using NUnit.Framework;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Utils;

namespace OnlineMessenger.Domain.Tests.Unit.Utils
{

    public class ValidationExtensionsTests : TestsBase
    {
        [Test]
        public void GetValidationErrors_CalledOnInvalidObject_ReturnsCorrectErrorsList()
        {
            var user = new User
                       {
                           Name = string.Empty,
                           Email = "false email"
                       };

            var errors = user.GetValidationErrors();

            Assert.IsTrue(errors.Count == 3);
            Assert.IsTrue(errors[0].Name == "Name");
            Assert.IsTrue(errors[1].Name == "Name");
            Assert.IsTrue(errors[2].Name == "Email");
        }


    }
}
