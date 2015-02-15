using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Exceptions;
using OnlineMessenger.Domain.Services;
using OnlineMessenger.MvcServer.ViewModels;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable InconsistentNaming


namespace OnlineMessenger.MvcServer.Controllers
{
    [Authorize(Roles = Role.AdministratorRoleName)]
    public class UsersController : ControllerBase
    {
        #region private fields

        private readonly IAccountManger _accountManger;

        #endregion private fields

        #region constructor

        public UsersController(IAccountManger accountManger)
        {
            _accountManger = accountManger;
        }

        #endregion constructor

        #region actionresults

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        #endregion actionresults

        #region jsonresults

        [HttpPost]
        public JsonResult SearchUsers(int[] Id, string[] Name, DateTime? CreatedDate, string[] Email, string[] Roles, string orderBy, bool isAsc = false)
        {
            return Json(_accountManger.GetUsers(Id, Name, CreatedDate, Email, Roles, orderBy, isAsc)
                .Select(Mapper.Map<UserTableItemViewModel>));
        }

        [HttpPost]
        public JsonResult GetUsersByIds(int[] userIds)
        {
            var users = new List<UserEditViewModel>();
            if (userIds != null)
                users.AddRange(_accountManger.GetUsersByIds(userIds).Select(Mapper.Map<UserEditViewModel>));
            return Json(new
                        {
                            Roles = _accountManger.GetRoles().Select(x => new { x.Name, x.Id }),
                            Users = users
                        });
        }

        [HttpPost]
        public JsonResult DeleteUsers(int[] ids)
        {
            _accountManger.DeleteUsers(ids);
            return Json(new { Success = true });
        }


        [HttpPost]
        public JsonResult SaveOrEditUsers(UserEditViewModel[] users)
        {
            try
            {
                var updatedAndCreated = _accountManger.UpdateOrCreate(users.Select(x => new UserPasswordPair(Mapper.Map<User>(x), x.IsChangePassword, x.Password)));
                return Json(new { Success = true, Items = updatedAndCreated.Select(Mapper.Map<UserEditViewModel>) });
            }
            catch (AggregateValidationException<User> exception)
            {
                return Json(new { Success = false, Errors = exception.EntityErrors });
            }
        }

        #endregion jsonresults

        #region protected methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _accountManger.Dispose();
        }

        #endregion protected methods

    }
}