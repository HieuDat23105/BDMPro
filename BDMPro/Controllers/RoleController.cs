using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BDMPro.Models;
using BDMPro.Resources;
using BDMPro.Utils;
using BDMPro.Data;
using System.Threading.Tasks;
using BDMPro.Services;
using System.Reflection;
using BDMPro.Data;
using BDMPro.Models;
using BDMPro.Services;
using BDMPro.Utils;

namespace BDMPro.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private DefaultDBContext db;
        private Util util;
        private readonly UserManager<AspNetUsers> _userManager;
        private ErrorLoggingService _logger;

        public RoleController(DefaultDBContext db, Util util, UserManager<AspNetUsers> userManager, ErrorLoggingService logger)
        {
            this.db = db;
            this.util = util;
            _userManager = userManager;
            _logger = logger;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RoleManagement, "true", "", "", "")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewRole([FromBody] dynamic requestData)
        {
            try
            {
                string sort = requestData.sort?.Value;
                int? size = (int.TryParse(requestData.size.Value, out int parsedSize)) ? parsedSize : null;
                string search = requestData.search?.Value;
                int? pg = (int.TryParse(requestData.pg.Value, out int parsedPg)) ? parsedPg : 1;

                List<ColumnHeader> headers = new List<ColumnHeader>();
                if (string.IsNullOrEmpty(sort))
                {
                    sort = RoleListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(RoleListConfig.DefaultColumnHeaders, sort);
                var list = ReadSystemRoles();
                string searchMessage = RoleListConfig.SearchMessage;
                list = RoleListConfig.PerformSearch(list, search);
                list = RoleListConfig.PerformSort(list, sort);
                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;
                int? total = list.Count();
                int? defaultSize = RoleListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;
                PaginatedList<SystemRoleViewModel> result = await PaginatedList<SystemRoleViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, searchMessage);
                return PartialView("~/Views/Role/_MainList.cshtml", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return PartialView("~/Views/Shared/Error.cshtml", null);
        }

        public IQueryable<SystemRoleViewModel> ReadSystemRoles()
        {
            try
            {
                var list = from t1 in db.AspNetRoles.AsNoTracking()
                           orderby t1.Name
                           select new SystemRoleViewModel
                           {
                               Id = t1.Id,
                               Name = t1.Name,
                               SystemDefault = t1.SystemDefault
                           };
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return null;
        }

        public SystemRoleViewModel GetViewModel(string Id, string type)
        {
            SystemRoleViewModel model = new SystemRoleViewModel();
            try
            {
                AspNetRoles roles = db.AspNetRoles.Where(a => a.Id == Id).FirstOrDefault();
                model.Id = roles.Id;
                model.Name = roles.Name;
                model.CreatedBy = roles.CreatedBy;
                model.CreatedOn = roles.CreatedOn;
                model.ModifiedBy = roles.ModifiedBy;
                model.ModifiedOn = roles.ModifiedOn;
                model.IsoUtcCreatedOn = roles.IsoUtcCreatedOn;
                model.IsoUtcModifiedOn = roles.IsoUtcModifiedOn;
                model.SystemDefault = roles.SystemDefault;
                if (type == "View")
                {
                    model.CreatedAndModified = util.GetCreatedAndModified(model.CreatedBy, model.IsoUtcCreatedOn, model.ModifiedBy, model.IsoUtcModifiedOn);
                }

                string dashboardModuleId = db.Modules.Where(a => a.Name == "Dashboard").Select(a => a.Id).FirstOrDefault();
                model.DashboardPermission = GetPermission(roles.Id, dashboardModuleId);

                string userStatusModuleId = db.Modules.Where(a => a.Name == "User Status").Select(a => a.Id).FirstOrDefault();
                model.UserStatusPermission = GetPermission(roles.Id, userStatusModuleId);

                string userAttachmentTypeModuleId = db.Modules.Where(a => a.Name == "User Attachment Type").Select(a => a.Id).FirstOrDefault();
                model.UserAttachmentTypePermission = GetPermission(roles.Id, userAttachmentTypeModuleId);

                string roleManagementModuleId = db.Modules.Where(a => a.Name == "Role Management").Select(a => a.Id).FirstOrDefault();
                model.RoleManagementPermission = GetPermission(roles.Id, roleManagementModuleId);

                string userManagementModuleId = db.Modules.Where(a => a.Name == "User Management").Select(a => a.Id).FirstOrDefault();
                model.UserManagementPermission = GetPermission(roles.Id, userManagementModuleId);

                string supplierManagementModuleId = db.Modules.Where(a => a.Name == "Supplier Management").Select(a => a.Id).FirstOrDefault();
                model.SupplierManagementPermission = GetPermission(roles.Id, supplierManagementModuleId);

                string deviceManagementModuleId = db.Modules.Where(a => a.Name == "Device Management").Select(a => a.Id).FirstOrDefault();
                model.DeviceManagementPermission = GetPermission(roles.Id, deviceManagementModuleId);

                string deviceTypeModuleId = db.Modules.Where(a => a.Name == "Device Type").Select(a => a.Id).FirstOrDefault();
                model.DeviceTypePermission = GetPermission(roles.Id, deviceTypeModuleId);

                string repairManagementModuleId = db.Modules.Where(a => a.Name == "Repair Management").Select(a => a.Id).FirstOrDefault();
                model.RepairManagementPermission = GetPermission(roles.Id, repairManagementModuleId);

                string repairTypeModuleId = db.Modules.Where(a => a.Name == "Repair Type").Select(a => a.Id).FirstOrDefault();
                model.RepairTypePermission = GetPermission(roles.Id, repairTypeModuleId);

                string statisticalModuleId = db.Modules.Where(a => a.Name == "Statistical").Select(a => a.Id).FirstOrDefault();
                model.StatisticalPermission = GetPermission(roles.Id, statisticalModuleId);

                string loginHistoryModuleId = db.Modules.Where(a => a.Name == "Login History").Select(a => a.Id).FirstOrDefault();
                model.LoginHistoryPermission = GetPermission(roles.Id, loginHistoryModuleId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        public Permission GetPermission(string roleId, string moduleId)
        {
            Permission permission = new Permission();
            try
            {
                permission.ViewPermission = new ViewPermission();
                permission.ViewPermission.Type = "View";
                permission.AddPermission = new AddPermission();
                permission.AddPermission.Type = "Add";
                permission.EditPermission = new EditPermission();
                permission.EditPermission.Type = "Edit";
                permission.DeletePermission = new DeletePermission();
                permission.DeletePermission.Type = "Delete";

                RoleModulePermission roleModulePermission = db.RoleModulePermissions.Where(a => a.RoleId == roleId && a.ModuleId == moduleId).FirstOrDefault();
                if (roleModulePermission != null)
                {
                    permission.ViewPermission.IsSelected = roleModulePermission.ViewRight;
                    permission.AddPermission.IsSelected = roleModulePermission.AddRight;
                    permission.EditPermission.IsSelected = roleModulePermission.EditRight;
                    permission.DeletePermission.IsSelected = roleModulePermission.DeleteRight;
                }
                else
                {
                    permission.ViewPermission.IsSelected = false;
                    permission.AddPermission.IsSelected = false;
                    permission.EditPermission.IsSelected = false;
                    permission.DeletePermission.IsSelected = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return permission;
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewUserInRole(string id, [FromBody] dynamic requestData)
        {
            try
            {
                string sort = requestData.sort?.Value;
                int? size = (int.TryParse(requestData.size.Value, out int parsedSize)) ? parsedSize : null;
                string search = requestData.search?.Value;
                int? pg = (int.TryParse(requestData.pg.Value, out int parsedPg)) ? parsedPg : 1;

                List<ColumnHeader> headers = new List<ColumnHeader>();
                if (string.IsNullOrEmpty(sort))
                {
                    sort = UserInRoleListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(UserInRoleListConfig.DefaultColumnHeaders, sort);
                var list = ReadUserInRole(id);
                string searchMessage = UserInRoleListConfig.SearchMessage;
                list = UserInRoleListConfig.PerformSearch(list, search);
                list = UserInRoleListConfig.PerformSort(list, sort);
                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;
                int? total = list.Count();
                int? defaultSize = UserInRoleListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;
                PaginatedList<UserInRoleViewModel> result = await PaginatedList<UserInRoleViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, searchMessage);
                return PartialView("~/Views/Role/_UserInRoleList.cshtml", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return PartialView("~/Views/Shared/Error.cshtml", null);
        }

        public IQueryable<UserInRoleViewModel> ReadUserInRole(string id)
        {
            if (id != null)
            {
                try
                {
                    var userList = from t1 in db.AspNetUserRoles.AsNoTracking()
                                   let t2 = db.UserProfiles.FirstOrDefault(up => up.AspNetUserId == t1.UserId)
                                   let t3 = db.AspNetUsers.FirstOrDefault(u => u.Id == t1.UserId)
                                   where t1.RoleId == id
                                   select new UserInRoleViewModel
                                   {
                                       FullName = t2.FullName,
                                       Username = t3.UserName,
                                       UserProfileId = t2.Id
                                   };
                    return userList;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                }
            }
            return null;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RoleManagement, "", "true", "true", "")]
        public IActionResult Edit(string Id)
        {
            SystemRoleViewModel model = new SystemRoleViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RoleManagement, "true", "", "", "")]
        public IActionResult ViewRecord(string Id)
        {
            SystemRoleViewModel model = new SystemRoleViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(SystemRoleViewModel model)
        {
            try
            {
                ValidateModel(model);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                SaveRecord(model);
                TempData["NotifySuccess"] = "Record saved successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index");
        }

        public void ValidateModel(SystemRoleViewModel model)
        {
            if (model != null)
            {
                bool rolesExisted = false;
                if (model.Id != null)
                {
                    rolesExisted = db.AspNetRoles.Where(a => a.Name == model.Name && a.Id != model.Id).Any();
                }
                else
                {
                    rolesExisted = db.AspNetRoles.Where(a => a.Name == model.Name).Select(a => a.Id).Any();
                }

                if (rolesExisted == true)
                {
                    ModelState.AddModelError("Name", Resource.RoleNameAlreadyExist);
                }
            }
        }

        public void SaveRecord(SystemRoleViewModel model)
        {
            if (model != null)
            {
                DateTime? now = util.GetSystemTimeZoneDateTimeNow();
                string utcNow = util.GetIsoUtcNow();
                //edit
                if (model.Id != null)
                {
                    AspNetRoles roles = db.AspNetRoles.Where(a => a.Id == model.Id).FirstOrDefault();
                    roles.Name = model.Name;
                    roles.NormalizedName = model.Name.ToUpper();
                    roles.ModifiedBy = _userManager.GetUserId(User);
                    roles.ModifiedOn = now;
                    roles.IsoUtcModifiedOn = utcNow;
                    db.Entry(roles).State = EntityState.Modified;
                    db.SaveChanges();

                    if (model.UserStatusPermission != null)
                    {
                        SaveRoleModulePermission(model.UserStatusPermission, roles.Id, "User Status", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.UserStatusPermission, roles.Id, "User Status", true, false);
                    }

                    if (model.UserAttachmentTypePermission != null)
                    {
                        SaveRoleModulePermission(model.UserAttachmentTypePermission, roles.Id, "User Attachment Type", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.UserAttachmentTypePermission, roles.Id, "User Attachment Type", true, false);
                    }

                    if (model.RoleManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.RoleManagementPermission, roles.Id, "Role Management", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.RoleManagementPermission, roles.Id, "Role Management", true, false);
                    }

                    if (model.UserManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.UserManagementPermission, roles.Id, "User Management", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.UserManagementPermission, roles.Id, "User Management", true, false);
                    }

                    if (model.SupplierManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.SupplierManagementPermission, roles.Id, "Supplier Management", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.SupplierManagementPermission, roles.Id, "Supplier Management", true, false);
                    }

                    if (model.DeviceManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.DeviceManagementPermission, roles.Id, "Device Management", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.DeviceManagementPermission, roles.Id, "Device Management", true, false);
                    }

                    if (model.DeviceTypePermission != null)
                    {
                        SaveRoleModulePermission(model.DeviceTypePermission, roles.Id, "Device Type Management", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.DeviceTypePermission, roles.Id, "Device Type Management", true, false);
                    }

                    if (model.RepairManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.RepairManagementPermission, roles.Id, "Repair Management", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.RepairManagementPermission, roles.Id, "Repair Management", true, false);
                    }

                    if (model.RepairTypePermission != null)
                    {
                        SaveRoleModulePermission(model.RepairTypePermission, roles.Id, "Repair Type Management", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.RepairTypePermission, roles.Id, "Repair Type Management", true, false);
                    }

                    if (model.StatisticalPermission != null)
                    {
                        SaveRoleModulePermission(model.StatisticalPermission, roles.Id, "Statistical", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.StatisticalPermission, roles.Id, "Statistical", true, false);
                    }

                    if (model.LoginHistoryPermission != null)
                    {
                        SaveRoleModulePermission(model.LoginHistoryPermission, roles.Id, "Login History", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.LoginHistoryPermission, roles.Id, "Login History", true, false);
                    }

                    if (model.DashboardPermission != null)
                    {
                        SaveRoleModulePermission(model.DashboardPermission, roles.Id, "Dashboard", false, false);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.DashboardPermission, roles.Id, "Dashboard", true, false);
                    }
                }
                else
                {
                    //new record

                    AspNetRoles roles = new AspNetRoles();
                    roles.Id = Guid.NewGuid().ToString();
                    roles.Name = model.Name;
                    roles.NormalizedName = model.Name.ToUpper();
                    roles.CreatedBy = _userManager.GetUserId(User);
                    roles.CreatedOn = now;
                    roles.IsoUtcCreatedOn = utcNow;
                    roles.SystemDefault = false; //only system admin and normal user roles are systemDefault = true, for all the new records added by user, set to false
                    db.AspNetRoles.Add(roles);
                    db.SaveChanges();

                    if (model.UserStatusPermission != null)
                    {
                        SaveRoleModulePermission(model.UserStatusPermission, roles.Id, "User Status", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.UserStatusPermission, roles.Id, "User Status", true, true);
                    }

                    if (model.UserAttachmentTypePermission != null)
                    {
                        SaveRoleModulePermission(model.UserAttachmentTypePermission, roles.Id, "User Attachment Type", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.UserAttachmentTypePermission, roles.Id, "User Attachment Type", true, true);
                    }

                    if (model.RoleManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.RoleManagementPermission, roles.Id, "Role Management", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.RoleManagementPermission, roles.Id, "Role Management", true, true);
                    }

                    if (model.UserManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.UserManagementPermission, roles.Id, "User Management", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.UserManagementPermission, roles.Id, "User Management", true, true);
                    }

                    if (model.SupplierManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.SupplierManagementPermission, roles.Id, "Supplier Management", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.SupplierManagementPermission, roles.Id, "Supplier Management", true, true);
                    }

                    if (model.DeviceManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.DeviceManagementPermission, roles.Id, "Device Management", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.DeviceManagementPermission, roles.Id, "Device Management", true, true);
                    }

                    if (model.DeviceTypePermission != null)
                    {
                        SaveRoleModulePermission(model.DeviceTypePermission, roles.Id, "Device Type Management", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.DeviceTypePermission, roles.Id, "Device Type Management", true, true);
                    }

                    if (model.RepairManagementPermission != null)
                    {
                        SaveRoleModulePermission(model.RepairManagementPermission, roles.Id, "Repair Management", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.RepairManagementPermission, roles.Id, "Repair Management", true, true);
                    }

                    if (model.RepairTypePermission != null)
                    {
                        SaveRoleModulePermission(model.RepairTypePermission, roles.Id, "Repair Type Management", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.RepairTypePermission, roles.Id, "Repair Type Management", true, true);
                    }

                    if (model.StatisticalPermission != null)
                    {
                        SaveRoleModulePermission(model.StatisticalPermission, roles.Id, "Statistical", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.StatisticalPermission, roles.Id, "Statistical", true, true);
                    }

                    if (model.LoginHistoryPermission != null)
                    {
                        SaveRoleModulePermission(model.LoginHistoryPermission, roles.Id, "Login History", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.LoginHistoryPermission, roles.Id, "Login History", true, true);
                    }

                    if (model.DashboardPermission != null)
                    {
                        SaveRoleModulePermission(model.DashboardPermission, roles.Id, "Dashboard", false, true);
                    }
                    else
                    {
                        SaveRoleModulePermission(model.DashboardPermission, roles.Id, "Dashboard", true, true);
                    }
                }
            }
        }

        public void SaveRoleModulePermission(Permission model, string roleId, string moduleName, bool setAllToFalse, bool addNewRecord)
        {
            DateTime? now = util.GetSystemTimeZoneDateTimeNow();
            string utcNow = util.GetIsoUtcNow();
            RoleModulePermission roleModulePermission = new RoleModulePermission();
            string moduleId = db.Modules.Where(a => a.Name == moduleName).Select(a => a.Id).FirstOrDefault();
            if (addNewRecord)
            {
                roleModulePermission.Id = Guid.NewGuid().ToString();
            }
            if (addNewRecord == false)
            {
                roleModulePermission = db.RoleModulePermissions.Where(a => a.RoleId == roleId && a.ModuleId == moduleId).FirstOrDefault();
            }
            if (setAllToFalse)
            {
                roleModulePermission.ViewRight = false;
                roleModulePermission.AddRight = false;
                roleModulePermission.EditRight = false;
                roleModulePermission.DeleteRight = false;
            }
            else
            {
                roleModulePermission.ViewRight = model.ViewPermission != null ? model.ViewPermission.IsSelected : false;
                roleModulePermission.AddRight = model.AddPermission != null ? model.AddPermission.IsSelected : false;
                roleModulePermission.EditRight = model.EditPermission != null ? model.EditPermission.IsSelected : false;
                roleModulePermission.DeleteRight = model.DeletePermission != null ? model.DeletePermission.IsSelected : false;
            }
            roleModulePermission.RoleId = roleId;
            roleModulePermission.ModuleId = moduleId;
            if (addNewRecord)
            {
                roleModulePermission.CreatedBy = _userManager.GetUserId(User);
                roleModulePermission.CreatedOn = now;
                roleModulePermission.IsoUtcCreatedOn = utcNow;
                db.RoleModulePermissions.Add(roleModulePermission);
            }
            else
            {
                roleModulePermission.ModifiedBy = _userManager.GetUserId(User);
                roleModulePermission.ModifiedOn = now;
                roleModulePermission.IsoUtcModifiedOn = utcNow;
                db.Entry(roleModulePermission).State = EntityState.Modified;
            }
            db.SaveChanges();
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RoleManagement, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    AspNetRoles roles = db.AspNetRoles.Where(a => a.Id == Id).FirstOrDefault();
                    if (roles != null)
                    {
                        db.AspNetRoles.Remove(roles);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                AspNetRoles roles = db.AspNetRoles.Where(a => a.Id == Id).FirstOrDefault();
                if (roles == null)
                {
                    TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
                if (util != null)
                {
                    util.Dispose();
                    util = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
