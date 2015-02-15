using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.IO;
using OnlineMessenger.Data.Ef.Utils;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Utils;

namespace OnlineMessenger.Data.Ef.Models
{
    public class AppDatabaseInitializer : IDatabaseInitializer<MessengerDbContext>
    {
        public void InitializeDatabase(MessengerDbContext context)
        {
            context.ClearData();
            context.SaveChanges();

            AddAdmin(context);
            AddAllUsers(context);
        }

        private static void AddAdmin(MessengerDbContext context)
        {
            var adminRole = new Role { Name = Role.AdministratorRoleName };
            context.Roles.Add(adminRole);
            var operatorRole = new Role { Name = Role.OperatorRoleName };
            context.Roles.Add(operatorRole);
            var user = new User { Name = "Admin", PasswordHash = PasswordHasher.HashPassword("123456"), Email = "Tes111t@mail.ru", Roles = new List<Role> { adminRole } };
            context.Users.Add(user);
            context.SaveChanges();
        }

        #region userNames

        private static string[] UserNames =
        {
            "Emily",
            "Denise",
            "Samuel",
            "William",
            "Arthur",
            "Judith",
            "Kathleen",
            "Martin",
            "Charles",
            "Stephen",
            "Jesse",
            "Chris",
            "Russell",
            "Jean",
            "David",
            "Edward",
            "Adam",
            "Bruce",
            "Ashley",
            "Betty",
            "Jose",
            "Nancy",
            "Craig",
            "Rachel",
            "Deborah",
            "Fred",
            "Helen",
            "Billy",
            "Jeremy",
            "Melissa",
            "Brian",
            "Walter",
            "Carl",
            "Matthew",
            "Justin",
            "Heather",
            "Bonnie",
            "Joshua",
            "Douglas",
            "Jeffrey",
            "Jennifer",
            "Margaret"
        };

        #endregion

        private static void AddAllUsers(MessengerDbContext context)
        {
            var rand = new Random();

            var operatorRole = context.Roles.Single(x => x.Name == Role.OperatorRoleName);

            var passwordHash = PasswordHasher.HashPassword("123456");
            var users =
                UserNames.ToList()
                    .Select((x, i) =>
                            {
                                var user = new User
                                           {
                                               Email = GetRandomEmail(),
                                               Name = x,
                                               PasswordHash = passwordHash
                                           };
                                user.Roles.Add(operatorRole);
                                return user;
                            }
                    );
            context.Users.AddRange(users);
            context.SaveChanges();

            var random = new Random();



            context.SaveChanges();
            var dbUsers = context.Users;

            var count = dbUsers.Count();
            var messages =
                File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath ?? Environment.CurrentDirectory, "Messages.txt"))
                    .Split(".".ToCharArray())
                    .Select(x => new Message
                                 {
                                     Date = DateTime.Now.AddDays(random.Next(-10,10)),
                                     From = dbUsers.ToList().ElementAt(rand.Next(random.Next(count))),
                                     Text = x
                                 });
            context.Messages.AddRange(messages);
            context.SaveChanges();


            foreach (var user in context.Users.ToList())
            {
                var visUsers = new int[random.Next(1, 5)].Select(x => GetRandomUser());
                user.Clients.AddRange(visUsers);
                user.VisibleClients.AddRange(visUsers);
                context.SaveChanges();
            }
        }

        private static string GetRandomEmail()
        {
            return string.Format("{0}@gmail.com", Path.GetRandomFileName());
        }

        private static int indexer = 0;
        private static readonly Random Random = new Random();
        private static User GetRandomUser()
        {
            indexer++;
            string userName = String.Format("User{0}", indexer);
            return new User
                   {
                       Email = GetRandomEmail(),
                       Name = userName,
                       CreatedDate = DateTime.Now.AddDays(Random.Next(-10, 10)),
                       PasswordHash = PasswordHasher.HashPassword("123456")
                   };
        }

    }
}
