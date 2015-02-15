using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Exceptions;
using OnlineMessenger.Domain.Extensions;
using OnlineMessenger.Domain.Infrastructure;
using OnlineMessenger.Domain.Utils;

// ReSharper disable  PossibleMultipleEnumeration
// ReSharper disable  ReturnTypeCanBeEnumerable.Local

namespace OnlineMessenger.Domain.Services
{
    public class AccountManager : IAccountManger
    {
        #region private fields

        private const string RegexLengthPattern = @"^[^.]{5,250}$";
        private const int NewId = 0;
        private readonly IUnitOfWork _unitOfWork;

        #endregion private fields

        public AccountManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region implementation of IAccountManger

        public bool CreateUser(User user, string password)
        {
            user.ThrowIfInvalid();
            user.PasswordHash = PasswordHasher.HashPassword(password);
            _unitOfWork.UserRepository.Add(user);
            _unitOfWork.Commit();
            return true;
        }

        public bool UpdateUserSettings(User user)
        {
            user.ThrowIfInvalid();
            var dbUser = _unitOfWork.UserRepository.GetUserByNameWithVisibleUsersAndRoles(user.Name);
            dbUser.VisualNotificationsEnabled = user.VisualNotificationsEnabled;
            dbUser.AudioNotificationsEnabled = user.AudioNotificationsEnabled;
            dbUser.Email = user.Email;
            _unitOfWork.Commit();
            return true;
        }

        public IEnumerable<Role> UpdateOrCreate(Role[] roles)
        {
            ValidateEntitiesThrowIfInvalid(() => ValidateRoles(roles));
            foreach (var role in roles)
            {
                if (role.Id == NewId)
                {
                    _unitOfWork.RoleRepository.Add(role);
                }
                else
                {
                    var dbRole = _unitOfWork.RoleRepository.GetRoleByIds(role.Id);
                    dbRole.Name = role.Name;
                }

            }
            _unitOfWork.Commit();
            return roles;
        }

        public IEnumerable<User> UpdateOrCreate(IEnumerable<UserPasswordPair> userPasswordPairs)
        {
            userPasswordPairs = SetUsersValues(userPasswordPairs);
            ValidateEntitiesThrowIfInvalid(() => ValidateEntities(userPasswordPairs));
            foreach (var pair in userPasswordPairs)
            {
                if (pair.IsChangePassword || pair.User.Id == NewId)
                    pair.User.PasswordHash = PasswordHasher.HashPassword(pair.Password);

                if (pair.User.Id == NewId)
                {
                    _unitOfWork.UserRepository.Add(pair.User);
                }
                else
                {
                    if (pair.IsChangePassword)
                    {
                        pair.User.PasswordHash = PasswordHasher.HashPassword(pair.Password);
                    }
                    else
                    {
                        _unitOfWork.UserRepository.Update(pair.User);
                    }
                }
            }
            _unitOfWork.Commit();
            return userPasswordPairs.Select(x => x.User);
        }


        public void ChangePassword(string userName, string oldPassword, string password, string passwordConfirm)
        {
            if (CheckPassword(userName, oldPassword) == null)
                throw new ValidationException("CurrentPassword", "Current password or user name is wrong");
            if (password != passwordConfirm)
                throw new ValidationException("PasswordConfirm", "Password confirmation is not equal to new password");
            SetPassword(userName, password);
        }

        public bool SetPassword(string userName, string password)
        {
            ValidatePasswordThrowIfInvalid(password);
            var user = _unitOfWork.UserRepository.GetUserByName(userName);
            user.PasswordHash = PasswordHasher.HashPassword(password);
            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Commit();
            return true;
        }

        public User CheckPassword(string userName, string password)
        {
            var user = _unitOfWork.UserRepository.GetUserByNameWithRoles(userName);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
                return null;
            return user;
        }


        public IEnumerable<User> GetUsers(int[] ids = null, string[] userNames = null, DateTime? fromDate = null,
            string[] emails = null, string[] roles = null, string orderBy = null, bool isAsc = false)
        {
            var query = _unitOfWork.UserRepository.Query(x => x.Roles, x => x.FromMessages, x => x.ToMessages);
            if (ids != null && ids.Length > 0)
                query = query.Where(x => ids.Contains(x.Id));
            if (fromDate != null)
                query = query.Where(x => x.CreatedDate >= fromDate);
            if (userNames != null && userNames.Length > 0)
                query = query.Where(x => userNames.Select(s => s.ToLower()).Contains(x.Name.ToLower()));
            if (emails != null && emails.Length > 0)
                query = query.Where(x => emails.Select(s => s.ToLower()).Contains(x.Email.ToLower()));
            if (roles != null && roles.Length > 0)
                query = query.Where(x => roles.Select(s => s.ToLower()).Intersect(x.Roles.Select(r => r.Name.ToLower())).Any());
            if (orderBy == "Roles")
                orderBy = "Roles.Count";
            if (orderBy == "MessagesCount")
                orderBy = "ToMessages.Count";
            if (!string.IsNullOrEmpty(orderBy))
                query = query.OrderBy(orderBy, isAsc);
            return query.ToList();
        }

        public IEnumerable<Role> GetRoles(int[] ids = null, string[] names = null, string[] users = null, string orderBy = null, bool isAsc = false)
        {
            var query = _unitOfWork.RoleRepository.Query(x => x.Users);
            if (ids != null && ids.Length > 0)
                query = query.Where(x => ids.Contains(x.Id));
            if (names != null && names.Length > 0)
                query = query.Where(x => names.Select(s => s.ToLower()).Contains(x.Name.ToLower()));
            if (users != null && users.Length > 0)
                query = query.Where(x => users.Select(s => s.ToLower()).Intersect(x.Users.Select(r => r.Name.ToLower())).Any());
            if (orderBy == "UsersCount")
                orderBy = "Users.Count";
            if (!string.IsNullOrEmpty(orderBy))
                query = query.OrderBy(orderBy, isAsc);
            return query.ToList();
        }

        public void DeleteUsers(int[] ids)
        {
            _unitOfWork.UserRepository
                .Delete(_unitOfWork.UserRepository.GetUsersByIds(ids).ToArray());
            _unitOfWork.Commit();
        }

        public void DeleteRoles(int[] ids)
        {
            _unitOfWork.RoleRepository
                .Delete(_unitOfWork.RoleRepository.GetRolesByIds(ids).ToArray());
            _unitOfWork.Commit();
        }

        public User GetUserByName(string name)
        {
            return _unitOfWork.UserRepository.GetUserByNameWithVisibleUsersAndRoles(name);
        }


        public IEnumerable<Role> GetRolesByIds(int[] ids)
        {
            return _unitOfWork.RoleRepository.GetRolesByIds(ids);
        }

        public byte[] GetUserAvatar(string userName)
        {
            var user = _unitOfWork.UserRepository.GetUserWithDataByName(userName);
            return user.UserData == null ? null : user.UserData.AvatarImage;
        }

        public void UploadUserAvatar(string userName, byte[] avatarData)
        {
            var resizedImage = ImageTools.ResizeImage(avatarData);
            var user = _unitOfWork.UserRepository.Query(x => x.UserData).Single(x => x.Name == userName);
            var userData = new UserData { AvatarImage = resizedImage, User = user };
            _unitOfWork.UserDataRepository.Add(userData);
            _unitOfWork.Commit();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        #endregion implementation of IAccountManger

        #region private methods

        public IEnumerable<UserPasswordPair> SetUsersValues(IEnumerable<UserPasswordPair> userPasswordPairs)
        {
            var dbUsers = _unitOfWork.UserRepository.GetUsersByIdsWithRoles(userPasswordPairs.Select(x => x.User.Id).ToArray());
            var mappedUsers = new List<UserPasswordPair>();
            foreach (var pair in userPasswordPairs)
            {
                var sourceUser = pair.User;
                var destinationUser = dbUsers.SingleOrDefault(x => x.Id == sourceUser.Id) ?? new User();
                var dbRoles = _unitOfWork.RoleRepository.GetRolesByIds(sourceUser.Roles.Select(x => x.Id).ToArray());
                destinationUser.Name = sourceUser.Name;
                destinationUser.Email = sourceUser.Email;

                destinationUser.Roles = dbRoles.ToList();

                mappedUsers.Add(new UserPasswordPair(destinationUser, pair.IsChangePassword, pair.Password));
            }
            return mappedUsers;
        }
        
        public IEnumerable<User> GetUsersByIds(int[] ids)
        {
            return _unitOfWork.UserRepository.Query(x => x.Roles).Where(x => ids.Contains(x.Id));
        }

        #region validtion methods

        private List<EntityErrors<Role>> ValidateRoles(IEnumerable<Role> roles)
        {
            var entityErrorses = new List<EntityErrors<Role>>();
            foreach (var role in roles)
            {
                var validationErrors = new List<ValidationError>();
                validationErrors.AddRange(role.GetValidationErrors());
                if (!IsRoleNameUnique(role))
                {
                    validationErrors.Add(new ValidationError("Name", "Role name is not unique"));
                }
                entityErrorses.Add(new EntityErrors<Role>(role, validationErrors));
            }
            return entityErrorses;
        }

        private bool IsRoleNameUnique(Role role)
        {
            var dbUser = _unitOfWork.RoleRepository.GetRoleByName(role.Name);
            if (dbUser == null || dbUser.Id == role.Id)
                return true;
            return false;
        }

        private static void ValidatePasswordThrowIfInvalid(string password)
        {
            if (!Regex.IsMatch(password, RegexLengthPattern))
                throw new ValidationException("Password", "Field cannot be empty, it must be in range from 5 to 250 signs");
        }

        private void ValidateEntitiesThrowIfInvalid<T>(Func<List<EntityErrors<T>>> func) where T : new()
        {
            var validations = func();
            if (validations.Any(x => x.IsErrors))
                throw new AggregateValidationException<T>(validations);
        }

        private List<EntityErrors<User>> ValidateEntities(IEnumerable<UserPasswordPair> userPasswordPairs)
        {
            var validations = new List<EntityErrors<User>>();
            foreach (var pair in userPasswordPairs)
            {
                var entityErrors = new List<ValidationError>();
                if (pair.IsChangePassword || pair.User.Id == NewId)
                    entityErrors.AddRange(ValidateUserAndPassword(pair.User, pair.Password));
                else
                    entityErrors.AddRange(ValidateUser(pair.User));
                validations.Add(new EntityErrors<User>(pair.User, entityErrors));
            }
            return validations;
        }

        private List<ValidationError> ValidateUserAndPassword(User user, string password)
        {
            var userErrors = ValidateUser(user);
            userErrors.AddRange(ValidatePassword(password));
            return userErrors;
        }

        private List<ValidationError> ValidateUser(User user)
        {
            var userErrors = new List<ValidationError>();
            if (!IsUserNameUnique(user))
                userErrors.Add(new ValidationError("Name", "User name exists"));
            if (!IsEmailUnique(user))
                userErrors.Add(new ValidationError("Email", "Email exists"));
            userErrors.AddRange(user.GetValidationErrors());
            return userErrors;
        }

        private static IEnumerable<ValidationError> ValidatePassword(string password)
        {
            var validationErrors = new List<ValidationError>();
            if (!Regex.IsMatch(password, RegexLengthPattern))
                validationErrors.Add(new ValidationError("Password", "Field cannot be empty, it must be in range from 5 to 250 signs"));
            return validationErrors;
        }

        #endregion validtion methods

        private bool IsEmailUnique(User user)
        {
            var dbUser = _unitOfWork.UserRepository.GetUserByEmail(user.Email);
            if (dbUser == null || dbUser.Id == user.Id)
                return true;
            return false;
        }

        private bool IsUserNameUnique(User user)
        {
            var dbUser = _unitOfWork.UserRepository.GetUserByName(user.Name);
            if (dbUser == null || dbUser.Id == user.Id)
                return true;
            return false;
        }

        #endregion


    }
}