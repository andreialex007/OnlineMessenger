using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Exceptions;
using OnlineMessenger.Domain.Services;
using OnlineMessenger.MvcServer.ViewModels;

namespace OnlineMessenger.MvcServer.Controllers
{
    [Authorize(Roles = Role.AdministratorRoleName)]
    public class RolesController : ControllerBase
    {
        private readonly IAccountManger _accountManager;

        public RolesController(IAccountManger accountManager)
        {
            _accountManager = accountManager;
        }

        #region ActionResults

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        #endregion ActionResults

        #region JsonResults

        public JsonResult SearchRoles(int[] Id, string[] Name, string orderBy, bool isAsc)
        {
            return Json(_accountManager.GetRoles(Id, Name, orderBy: orderBy, isAsc: isAsc)
                .Select(Mapper.Map<RoleTableItemViewModel>));
        }

        [HttpPost]
        public JsonResult DeleteRoles(int[] ids)
        {
            _accountManager.DeleteRoles(ids);
            return Json(new { Success = true });
        }

        [HttpPost]
        public JsonResult SaveOrEditRole(RoleEditViewModel[] roleEditViewModels)
        {
            try
            {
                var updatedAndCreated = _accountManager.UpdateOrCreate(roleEditViewModels.Select(Mapper.Map<Role>).ToArray());
                return Json(new { Success = true, Items = updatedAndCreated.Select(Mapper.Map<RoleEditViewModel>) });
            }
            catch (AggregateValidationException<Role> exception)
            {
                return Json(new { Success = false, Errors = exception.EntityErrors });
            }
        }

        [HttpPost]
        public JsonResult GetRolesById(int[] ids)
        {
            var roles = new List<RoleEditViewModel>();
            if (ids != null)
                roles.AddRange(_accountManager.GetRolesByIds(ids).Select(Mapper.Map<RoleEditViewModel>));
            return Json(new { Roles = roles });
        }

        #endregion JsonResults

        #region protected methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _accountManager.Dispose();
        }

        #endregion protected methods

    }
}