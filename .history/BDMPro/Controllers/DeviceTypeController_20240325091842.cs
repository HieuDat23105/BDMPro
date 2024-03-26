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
                var list = ReadDeviceTypes();
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

        public IQueryable<DeviceTypeViewModel> ReadDeviceTypes()
        {
            var result = from t1 in db.DeviceTypes.AsNoTracking()
                         select new DeviceTypeViewModel
                         {
                             DeviceTypeId = t1.DeviceTypeId,
                             TypeName = t1.TypeName,
                             TypeSymbol = t1.TypeSymbol,
                             Notes = t1.Notes,
                         };
            return result;
        }

        public DeviceTypeViewModel GetViewModel(string Id, string type)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            try
            {
                DeviceType deviceType = db.DeviceTypes.Where(a => a.DeviceTypeId == Id).FirstOrDefault();
                model.DeviceTypeId = deviceType.DeviceTypeId;
                model.TypeSymbol = deviceType.TypeSymbol;
                model.TypeName = deviceType.TypeName;
                model.IsoUtcCreatedOn = deviceType.IsoUtcCreatedOn;
                model.IsoUtcModifiedOn = deviceType.IsoUtcModifiedOn;
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
        public IActionResult Edit(string Id)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            else
            {
                string maxOrder = db.DeviceTypes
                    .Where(a => a.TypeSymbol.All(char.IsLetter))
                    .Select(a => a.TypeSymbol)
                    .OrderByDescending(a => a[0])
                    .FirstOrDefault();

                model.TypeSymbol = maxOrder;
            }
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserStatus, "true", "", "", "")]
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
        if (model.DeviceTypeId != null)
        {
            duplicated = db.DeviceTypes.Where(a => a.TypeName == model.TypeName && a.DeviceTypeId != model.DeviceTypeId).Any();
        }
        else
        {
            duplicated = db.DeviceTypes.Where(a => a.TypeName == model.TypeName).Any();
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
                    deviceType.TypeSymbol = sWhitespace.Replace(model.TypeSymbol, "");
                    deviceType.TypeName = model.TypeName;
                    deviceType.IsDeleted = false;
                    deviceType.ModifiedBy = _userManager.GetUserId(User);
                    deviceType.ModifiedOn = util.GetSystemTimeZoneDateTimeNow();
                    deviceType.IsoUtcModifiedOn = util.GetIsoUtcNow();
                    db.Entry(deviceType).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //bản ghi mới
                else
                {
                    DeviceType deviceType = new DeviceType();
                    deviceType.DeviceTypeId = Guid.NewGuid().ToString();
                    deviceType.Ty = sWhitespace.Replace(model.DisplayName, "");
                    deviceType.DisplayName = model.DisplayName;
                    deviceType.OptionOrder = model.OptionOrder;
                    deviceType.Type = "UserStatus";
                    deviceType.Status = "Active";
                    deviceType.CreatedBy = _userManager.GetUserId(User);
                    deviceType.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                    deviceType.IsoUtcCreatedOn = util.GetIsoUtcNow();
                    deviceType.SystemDefault = false;
                    db.GlobalOptionSets.Add(deviceType);
                    db.SaveChanges();
                }
            }
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserStatus, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    GlobalOptionSet globalOptionSet = db.GlobalOptionSets.Where(a => a.Id == Id).FirstOrDefault();
                    if (globalOptionSet != null)
                    {
                        db.GlobalOptionSets.Remove(globalOptionSet);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                GlobalOptionSet globalOptionSet = db.GlobalOptionSets.Where(a => a.Id == Id).FirstOrDefault();
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