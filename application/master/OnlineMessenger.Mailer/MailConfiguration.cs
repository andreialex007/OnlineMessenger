namespace OnlineMessenger.Mailer
{
    public class MailConfiguration
    {
        public string Subject { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string FromPassword { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool Ssl { get; set; }
    }
}
