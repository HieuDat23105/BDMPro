using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
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

namespace BDMPro.Controllers
{
    [Authorize]
    public class DeviceTypeController : Controller
    {
        private DefaultDBContext db;
        private Util util;
        private readonly UserManager<AspNetUsers> _userManager;
        private ErrorLoggingService _logger;
        public DeviceTypeController(DefaultDBContext db, Util util, UserManager<AspNetUsers> userManager, ErrorLoggingService logger)
        {
            this.db = db;
            this.util = util;
            _userManager = userManager;
            _logger = logger;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceTypeManagement, "true", "", "", "")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewDeviceType([FromBody] dynamic requestData)
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
                    sort = DeviceTypeListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(DeviceTypeListConfig.DefaultColumnHeaders, sort);
                var list = ReadDeviceTypees();
                string searchMessage = DeviceTypeListConfig.SearchMessage;
                list = DeviceTypeListConfig.PerformSearch(list, search);
                list = DeviceTypeListConfig.PerformSort(list, sort);
                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;
                int? total = list.Count();
                int? defaultSize = DeviceTypeListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;
                PaginatedList<DeviceTypeViewModel> result = await PaginatedList<DeviceTypeViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, searchMessage);
                return PartialView("~/Views/DeviceType/_MainList.cshtml", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return PartialView("~/Views/Shared/Error.cshtml", null);
        }

        public IQueryable<DeviceTypeViewModel> ReadDeviceTypees()
        {
            var result = from t1 in db.DeviceTypes.AsNoTracking()
                         where t1.Type == "DeviceType" && t1.Status == "Active"
                         select new DeviceTypeViewModel
                         {
                             Id = t1.Id,
                             Code = t1.Code,
                             DisplayName = t1.DisplayName,
                             OptionOrder = t1.OptionOrder,
                             SystemDefault = t1.SystemDefault
                         };
            return result;
        }

        public DeviceTypeViewModel GetViewModel(string Id, string type)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            try
            {
                DeviceType globalOptionSet = db.DeviceTypes.Where(a => a.Id == Id).FirstOrDefault();
                model.Id = globalOptionSet.Id;
                model.Code = globalOptionSet.Code;
                model.DisplayName = globalOptionSet.DisplayName;
                model.Status = globalOptionSet.Status;
                model.OptionOrder = globalOptionSet.OptionOrder;
                model.IsoUtcCreatedOn = globalOptionSet.IsoUtcCreatedOn;
                model.IsoUtcModifiedOn = globalOptionSet.IsoUtcModifiedOn;
                model.SystemDefault = globalOptionSet.SystemDefault;
                if (type == "View")
                {
                    model.CreatedAndModified = util.GetCreatedAndModified(globalOptionSet.CreatedBy, globalOptionSet.IsoUtcCreatedOn, globalOptionSet.ModifiedBy, globalOptionSet.IsoUtcModifiedOn);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceTypeManagement, "", "true", "true", "")]
        public IActionResult Edit(string Id)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            else
            {
                //hiển thị theo thứ tự
                int? maxOrder = db.DeviceTypes.Where(a => a.Type == "DeviceType" && a.Status == "Active").Select(a => a.OptionOrder).OrderByDescending(a => a.Value).FirstOrDefault();
                model.OptionOrder = maxOrder + 1;
            }
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceTypeManagement, "true", "", "", "")]
        public IActionResult ViewRecord(string Id)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(DeviceTypeViewModel model)
        {
            try
            {
                ValidateModel(model);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                SaveRecord(model);
                TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index");
        }

        public void ValidateModel(DeviceTypeViewModel model)
        {
            if (model != null)
            {
                bool duplicated = false;
                if (model.Id != null)
                {
                    duplicated = db.DeviceTypes.Where(a => a.DisplayName == model.DisplayName && a.Type == "DeviceType" && a.Id != model.Id).Any();
                }
                else
                {
                    duplicated = db.DeviceTypes.Where(a => a.DisplayName == model.DisplayName && a.Type == "DeviceType").Select(a => a.Id).Any();
                }

                if (duplicated == true)
                {
                    ModelState.AddModelError("DisplayName", Resource.DeviceTypeNameAlreadyExist);
                }
            }
        }

        public void SaveRecord(DeviceTypeViewModel model)
        {
            if (model != null)
            {
                Regex sWhitespace = new Regex(@"\s+");
                //edit
                if (model.Id != null)
                {
                    DeviceType deviceType = db.DeviceTypes.Where(a => a.TId == model.Id).FirstOrDefault();
                    deviceType.TypeName = model.TypeName;
                    db.Entry(deviceType).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //bản ghi mới
                else
                {
                    DeviceType globalOptionSet = new DeviceType();
                    globalOptionSet.Id = Guid.NewGuid().ToString();
                    globalOptionSet.Code = sWhitespace.Replace(model.DisplayName, "");
                    globalOptionSet.DisplayName = model.DisplayName;
                    globalOptionSet.OptionOrder = model.OptionOrder;
                    globalOptionSet.Type = "DeviceType";
                    globalOptionSet.Status = "Active";
                    globalOptionSet.CreatedBy = _userManager.GetUserId(User);
                    globalOptionSet.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                    globalOptionSet.IsoUtcCreatedOn = util.GetIsoUtcNow();
                    globalOptionSet.SystemDefault = false;
                    db.DeviceTypes.Add(globalOptionSet);
                    db.SaveChanges();
                }
            }
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceTypeManagement, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    DeviceType globalOptionSet = db.DeviceTypes.Where(a => a.Id == Id).FirstOrDefault();
                    if (globalOptionSet != null)
                    {
                        db.DeviceTypes.Remove(globalOptionSet);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                DeviceType globalOptionSet = db.DeviceTypes.Where(a => a.Id == Id).FirstOrDefault();
                if (globalOptionSet == null)
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