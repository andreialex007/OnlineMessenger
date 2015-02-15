using System;
using NUnit.Framework;

namespace OnlineMessenger.Mailer.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class MailerTests
    {
        //[Test]
        public void Send_CorrectConfig_MailSended()
        {
            //arrange
            var mailer = new Mailer();
            try
            {
                //act
                mailer.Send("user", "test text");
            }
            catch (Exception)
            {
                //assesrt
                Assert.Fail();
            }
        }


        [Test]
        [ExpectedException(typeof(System.Net.Mail.SmtpException))]
        public void Send_IncorrectPassword_ThrowsException()
        {
            var mailer = new Mailer();
            Mailer.Config.FromPassword = "test";
            mailer.Send("user", "test text");
        }

        [Test]
        [ExpectedException(typeof(System.Net.Mail.SmtpException))]
        public void Send_IncorrectSmptServer_ThrowsException()
        {
            var mailer = new Mailer();
            Mailer.Config.SmtpHost = "wrong";
            mailer.Send("user", "test text");
        }

        [Test]
        [ExpectedException(typeof(System.Net.Mail.SmtpException))]
        public void Send_IncorrectSsl_ThrowsException()
        {
            var mailer = new Mailer();
            Mailer.Config.Ssl = true;
            mailer.Send("user", "test text");
        }
    }
}
