namespace OnlineMessenger.Domain.Entities
{
    public class UserPasswordPair
    {

        public UserPasswordPair(User user, bool isChangePassword, string password)
        {
            User = user;
            IsChangePassword = isChangePassword;
            Password = password;
        }

        public User User { get; set; }
        public bool IsChangePassword { get; set; }
        public string Password { get; set; }
    }
}
