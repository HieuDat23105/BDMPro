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
                             DeviceTypeId = t1.DeviceTypeId,
                             Code = t1.Code,
                             TypeName = t1.TypeName,
                             OptionOrder = t1.OptionOrder,
                             SystemDefault = t1.SystemDefault
                         };
            return result;
        }

        public DeviceTypeViewModel GetViewModel(string DeviceTypeId, string type)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            try
            {
                DeviceType deviceType = db.DeviceTypes.Where(a => a.DeviceTypeId == DeviceTypeId).FirstOrDefault();
                model.DeviceTypeId = deviceType.DeviceTypeId;
                model.TypeName = deviceType.TypeName;
                model.Status = deviceType.Status;
                model.OptionOrder = deviceType.OptionOrder;
                model.IsoUtcCreatedOn = deviceType.IsoUtcCreatedOn;
                model.IsoUtcModifiedOn = deviceType.IsoUtcModifiedOn;
                model.SystemDefault = deviceType.SystemDefault;
                if (type == "View")
                {
                    model.CreatedAndModified = util.GetCreatedAndModified(deviceType.CreatedBy, deviceType.IsoUtcCreatedOn, deviceType.ModifiedBy, deviceType.IsoUtcModifiedOn);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceTypeManagement, "", "true", "true", "")]
        public IActionResult Edit(string DeviceTypeId)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            // if (DeviceTypeId != null)
            // {
                model = GetViewModel(DeviceTypeId, "Edit");
            // }
            // else
            // {
            //     //hiển thị theo thứ tự
            //     int? maxOrder = db.DeviceTypes.Where(a => a.TypeName == "DeviceType" && a. == "Active").Select(a => a.OptionOrder).OrderByDescending(a => a.Value).FirstOrDefault();
            //     model.OptionOrder = maxOrder + 1;
            // }
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceTypeManagement, "true", "", "", "")]
        public IActionResult ViewRecord(string DeviceTypeId)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            if (DeviceTypeId != null)
            {
                model = GetViewModel(DeviceTypeId, "View");
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
                if (model.DeviceTypeId != null)
                {
                    duplicated = db.DeviceTypes.Where(a => a.TypeName == model.TypeName && a.TypeName == "DeviceType" && a.DeviceTypeId != model.DeviceTypeId).Any();
                }
                else
                {
                    duplicated = db.DeviceTypes.Where(a => a.TypeName == model.TypeName && a.TypeName == "DeviceType").Select(a => a.DeviceTypeId).Any();
                }

                if (duplicated == true)
                {
                    ModelState.AddModelError("TypeName", Resource.DeviceTypeNameAlreadyExist);
                }
            }
        }

        public void SaveRecord(DeviceTypeViewModel model)
        {
            if (model != null)
            {
                Regex sWhitespace = new Regex(@"\s+");
                //edit
                if (model.DeviceTypeId != null)
                {
                    DeviceType deviceType = db.DeviceTypes.Where(a => a.DeviceTypeId == model.DeviceTypeId).FirstOrDefault();
                    deviceType.TypeName = model.TypeName;
                    db.Entry(deviceType).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //bản ghi mới
                else
                {
                    DeviceType deviceType = new DeviceType();
                    deviceType.DeviceTypeId = Guid.NewGuid().ToString();
                    deviceType.CreatedBy = _userManager.GetUserId(User);
                    deviceType.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                    deviceType.IsoUtcCreatedOn = util.GetIsoUtcNow();
                    db.DeviceTypes.Add(deviceType);
                    db.SaveChanges();
                }
            }
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceTypeManagement, "", "", "", "true")]
        public IActionResult Delete(string DeviceTypeId)
        {
            try
            {
                if (DeviceTypeId != null)
                {
                    DeviceType deviceType = db.DeviceTypes.Where(a => a.DeviceTypeId == DeviceTypeId).FirstOrDefault();
                    if (deviceType != null)
                    {
                        db.DeviceTypes.Remove(deviceType);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                DeviceType deviceType = db.DeviceTypes.Where(a => a.DeviceTypeId == DeviceTypeId).FirstOrDefault();
                if (deviceType == null)
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