using System;
using System.Collections.Generic;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Domain.Services
{
    /// <summary>
    /// Incapsulates different account operations, role, user management and account information retrieval
    /// </summary>
    public interface IAccountManger : IDisposable
    {
        /// <summary>
        /// Set password for user
        /// </summary>
        /// <returns></returns>
        bool SetPassword(string userName, string password);
        /// <summary>
        /// Create user with specific password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool CreateUser(User user, string password);
        /// <summary>
        /// Updates only user settings
        /// </summary>
        bool UpdateUserSettings(User user);
        /// <summary>
        /// Checks is password validity
        /// </summary>
        /// <returns></returns>
        User CheckPassword(string userName, string password);
        /// <summary>
        /// Performs complex query to users
        /// </summary>
        /// <returns></returns>
        IEnumerable<User> GetUsers(int[] ids = null, string[] userNames = null, DateTime? fromDate = null,
            string[] emails = null, string[] roles = null, string orderBy = null, bool isAsc = false);
        /// <summary>
        /// Performs complex query to users
        /// </summary>
        IEnumerable<Role> GetRoles(int[] ids = null, string[] names = null, string[] users = null, string
            orderBy = null, bool isAsc = false);
        /// <summary>
        /// Performs bunch update or create, if in list exists user with empty Id then add user to db, in other cases update user
        /// </summary>
        IEnumerable<User> UpdateOrCreate(IEnumerable<UserPasswordPair> userPasswordPairs);
        IEnumerable<User> GetUsersByIds(int[] ids);
        void DeleteUsers(int[] ids);
        void DeleteRoles(int[] ids);
        /// <summary>
        /// bunch update or create of roles list
        /// </summary>
        IEnumerable<Role> UpdateOrCreate(Role[] roles);
        IEnumerable<Role> GetRolesByIds(int[] ids);
        User GetUserByName(string name);
        /// <summary>
        /// Changes password, performs full password validation
        /// </summary>
        void ChangePassword(string userName, string oldPassword, string password, string passwordConfirm);
        void UploadUserAvatar(string userName, byte[] avatarData);
        byte[] GetUserAvatar(string userName);
    }
}