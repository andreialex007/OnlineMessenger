using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.Mailer
{
    public class Mailer : IMailer
    {
        private const string ConfigFileName = "MailConfiguration.xml";
        public static MailConfiguration Config;

        static Mailer()
        {
            var path = Path.Combine(
                AppDomain.CurrentDomain.RelativeSearchPath ??
                Environment.CurrentDirectory,
                ConfigFileName);
            var configuration = SerializationTools.FromXml<MailConfiguration>(File.ReadAllText(path));
            Config = configuration;
        }

        public void Send(string @from, string text)
        {
            var mail = new MailMessage();
            mail.To.Add(Config.To);
            mail.From = new MailAddress(Config.From);
            mail.Subject = string.Format(Config.Subject, @from);
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = text;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            var client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(Config.From, Config.FromPassword);
            client.Port = Config.SmtpPort;
            client.Host = Config.SmtpHost;
            client.EnableSsl = Config.Ssl; 
            client.Send(mail);
        }
    }
}
