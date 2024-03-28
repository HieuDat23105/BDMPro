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

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceType, "true", "", "", "")]
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
                var list = ReadDeviceTypeList();
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

        public IQueryable<DeviceTypeViewModel> ReadDeviceTypeList()
        {
            var result = from t1 in db.DeviceTypes.AsNoTracking()
                         select new DeviceTypeViewModel
                         {
                             DeviceTypeId = t1.DeviceTypeId,
                             TypeName = t1.TypeName,
                             TypeSymbol = t1.TypeSymbol,
                             Notes = t1.Notes,
                             CreatedOn = t1.CreatedOn,
                             IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                             DeviceCount = db.Devices.Where(a => a.DeviceTypeId == t1.DeviceTypeId).Count(),
                         };
            return result;
        }

        public DeviceTypeViewModel GetViewModel(string Id, string type)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            try
            {
                model = db.DeviceTypes
                    .Where(t1 => t1.DeviceTypeId == Id)
                    .Select(t1 => new DeviceTypeViewModel
                    {
                        DeviceTypeId = t1.DeviceTypeId,
                        TypeName = t1.TypeName,
                        TypeSymbol = t1.TypeSymbol,
                        Notes = t1.Notes,
                        CreatedBy = t1.CreatedBy,
                        ModifiedBy = t1.ModifiedBy,
                        CreatedOn = t1.CreatedOn,
                        ModifiedOn = t1.ModifiedOn,
                        IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                        IsoUtcModifiedOn = t1.IsoUtcModifiedOn,
                        DeviceCount = db.Devices.Count(a => a.DeviceTypeId == t1.DeviceTypeId)
                    })
                    .FirstOrDefault();

                if (type == "View")
                {
                    model.CreatedAndModified = util.GetCreatedAndModified(model.CreatedBy, model.IsoUtcCreatedOn, model.ModifiedBy, model.IsoUtcModifiedOn);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceType, "", "true", "true", "")]
        public IActionResult Edit(string Id)
        {
            DeviceTypeViewModel model = new DeviceTypeViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            else
            {
                var maxOrder = db.DeviceTypes
                    .Where(a => !string.IsNullOrEmpty(a.TypeSymbol))
                    .Select(a => a.TypeSymbol.Substring(0, 1))
                    .OrderByDescending(a => a)
                    .FirstOrDefault();

                if (maxOrder != default(string))
                {
                    char nextOrder = (char)(maxOrder[0] + 1);
                    model.TypeSymbol = nextOrder.ToString();
                }
            }
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceType, "true", "", "", "")]
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
        public async Task<IActionResult> Edit(DeviceTypeViewModel model)
        {
            try
            {
                ValidateModel(model);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                bool result = await SaveRecord(model);
                if (result == false)
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
                else
                {
                    TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
                }
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index", "devicetype");
        }

        public void ValidateModel(DeviceTypeViewModel model)
        {
            if (model != null)
            {
                bool duplicatedTypeName = false;
                if (model.DeviceTypeId == null)
                {
                    duplicatedTypeName = db.DeviceTypes.Any(a => a.TypeName == model.TypeName);
                }
                else
                {
                    duplicatedTypeName = db.DeviceTypes.Any(a => a.TypeName == model.TypeName && a.DeviceTypeId != model.DeviceTypeId);
                }
                if (duplicatedTypeName == true)
                {
                    ModelState.AddModelError("TypeName", Resource.DeviceTypeNameAlreadyExist);
                }

                bool duplicatedTypeSymbol = false;
                if (model.DeviceTypeId == null)
                {
                    duplicatedTypeSymbol = db.DeviceTypes.Any(a => a.TypeSymbol == model.TypeSymbol);
                }
                else
                {
                    duplicatedTypeSymbol = db.DeviceTypes.Any(a => a.TypeSymbol == model.TypeSymbol && a.DeviceTypeId != model.DeviceTypeId);
                }
                if (duplicatedTypeSymbol == true)
                {
                    ModelState.AddModelError("TypeSymbol", Resource.DeviceTypeSymbolAlreadyExist);
                }

                if (model.Notes != null && model.Notes.Length > 500) //độ dài tối đa là 500
                {
                    ModelState.AddModelError("Notes", Resource.DeviceTypeNotesTooLong);
                }
            }
        }

        public void AssignDeviceTypeValues(DeviceType deviceType, DeviceTypeViewModel model)
        {
            deviceType.TypeName = model.TypeName;
            deviceType.TypeSymbol = model.TypeSymbol;
            deviceType.Notes = model.Notes;
            if (model.DeviceTypeId == null)
            {
                deviceType.CreatedBy = model.CreatedBy;
                deviceType.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                deviceType.IsoUtcCreatedOn = util.GetIsoUtcNow();
            }
            else
            {
                deviceType.ModifiedBy = model.ModifiedBy;
                deviceType.ModifiedOn = util.GetSystemTimeZoneDateTimeNow();
                deviceType.IsoUtcModifiedOn = util.GetIsoUtcNow();
            }
        }

        public async Task<bool> SaveRecord(DeviceTypeViewModel model)
        {
            bool result = true;
            if (model != null)
            {
                string deviceTypeId = "";
                string type = "";
                try
                {
                    model.CreatedBy = _userManager.GetUserId(User);
                    model.ModifiedBy = _userManager.GetUserId(User);
                    if (model.DeviceTypeId != null)
                    {
                        type = "update";
                        DeviceType deviceType = db.DeviceTypes.FirstOrDefault(a => a.DeviceTypeId == model.DeviceTypeId);
                        AssignDeviceTypeValues(deviceType, model);
                        db.Entry(deviceType).State = EntityState.Modified;
                        db.SaveChanges();
                        deviceTypeId = deviceType.DeviceTypeId;
                    }
                    else
                    {
                        type = "create";
                        DeviceType deviceType = new DeviceType();
                        AssignDeviceTypeValues(deviceType, model);
                        deviceType.DeviceTypeId = Guid.NewGuid().ToString();
                        db.DeviceTypes.Add(deviceType);
                        db.SaveChanges();
                        deviceTypeId = deviceType.DeviceTypeId;
                    }
                }
                catch (Exception ex)
                {
                    if (type == "create")
                    {
                        if (!string.IsNullOrEmpty(deviceTypeId))
                        {
                            DeviceType deviceType = db.DeviceTypes.FirstOrDefault(a => a.DeviceTypeId == deviceTypeId);
                            if (deviceType != null)
                            {
                                db.DeviceTypes.Remove(deviceType);
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                    return false;
                }
            }
            return result;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.DeviceType, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    DeviceType deviceType = db.DeviceTypes.Where(a => a.DeviceTypeId == Id).FirstOrDefault();
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
                DeviceType deviceType = db.DeviceTypes.Where(a => a.DeviceTypeId == Id).FirstOrDefault();
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