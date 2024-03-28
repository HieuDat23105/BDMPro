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
    public class RepairTypeController : Controller
    {
        private DefaultDBContext db;
        private Util util;
        private readonly UserManager<AspNetUsers> _userManager;
        private ErrorLoggingService _logger;
        public RepairTypeController(DefaultDBContext db, Util util, UserManager<AspNetUsers> userManager, ErrorLoggingService logger)
        {
            this.db = db;
            this.util = util;
            _userManager = userManager;
            _logger = logger;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RepairType, "true", "", "", "")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewRepairType([FromBody] dynamic requestData)
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
                    sort = RepairTypeListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(RepairTypeListConfig.DefaultColumnHeaders, sort);
                var list = ReadRepairTypeList();
                string searchMessage = RepairTypeListConfig.SearchMessage;
                list = RepairTypeListConfig.PerformSearch(list, search);
                list = RepairTypeListConfig.PerformSort(list, sort);
                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;
                int? total = list.Count();
                int? defaultSize = RepairTypeListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;
                PaginatedList<RepairTypeViewModel> result = await PaginatedList<RepairTypeViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, searchMessage);
                return PartialView("~/Views/RepairType/_MainList.cshtml", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return PartialView("~/Views/Shared/Error.cshtml", null);
        }

        public IQueryable<RepairTypeViewModel> ReadRepairTypeList()
        {
            var result = from t1 in db.RepairTypes.AsNoTracking()
                         select new RepairTypeViewModel
                         {
                             RepairTypeId = t1.RepairTypeId,
                             RepairTypeName = t1.RepairTypeName,
                             RepairTypeSymbol = t1.RepairTypeSymbol,
                             Notes = t1.Notes,
                             CreatedOn = t1.CreatedOn,
                             IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                             RepairCount = db.RepairDetails.Where(a => a.RepairTypeId == t1.RepairTypeId).Count(),
                         };
            return result;
        }

        public RepairTypeViewModel GetViewModel(string Id, string type)
        {
            RepairTypeViewModel model = new RepairTypeViewModel();
            try
            {
                model = db.RepairTypes
                    .Where(t1 => t1.RepairTypeId == Id)
                    .Select(t1 => new RepairTypeViewModel
                    {
                        RepairTypeId = t1.RepairTypeId,
                        RepairTypeName = t1.RepairTypeName,
                        RepairTypeSymbol = t1.RepairTypeSymbol,
                        Notes = t1.Notes,
                        CreatedBy = t1.CreatedBy,
                        ModifiedBy = t1.ModifiedBy,
                        CreatedOn = t1.CreatedOn,
                        ModifiedOn = t1.ModifiedOn,
                        IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                        IsoUtcModifiedOn = t1.IsoUtcModifiedOn,
                        RepairCount = db.RepairDetails.Count(a => a.RepairTypeId == t1.RepairTypeId)
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

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RepairType, "", "true", "true", "")]
        public IActionResult Edit(string Id)
        {
            RepairTypeViewModel model = new RepairTypeViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            else
            {
                var maxOrder = db.RepairTypes
                    .Where(a => !string.IsNullOrEmpty(a.RepairTypeSymbol))
                    .Select(a => a.RepairTypeSymbol.Substring(0, 1))
                    .OrderByDescending(a => a)
                    .FirstOrDefault();

                if (maxOrder != default(string))
                {
                    char nextOrder = (char)(maxOrder[0] + 1);
                    model.RepairTypeSymbol = nextOrder.ToString();
                }
            }
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RepairType, "true", "", "", "")]
        public IActionResult ViewRecord(string Id)
        {
            RepairTypeViewModel model = new RepairTypeViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RepairTypeViewModel model)
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
            return RedirectToAction("index", "repairType");
        }

        public void ValidateModel(RepairTypeViewModel model)
        {
            if (model != null)
            {
                bool duplicatedTypeName = false;
                if (model.RepairTypeId == null)
                {
                    duplicatedTypeName = db.RepairTypes.Any(a => a.RepairTypeName == model.RepairTypeName);
                }
                else
                {
                    duplicatedTypeName = db.RepairTypes.Any(a => a.RepairTypeName == model.RepairTypeName && a.RepairTypeId != model.RepairTypeId);
                }
                if (duplicatedTypeName == true)
                {
                    ModelState.AddModelError("RepairTypeName", Resource.RepairTypeNameAlreadyExist);
                }

                bool duplicatedTypeSymbol = false;
                if (model.RepairTypeId == null)
                {
                    duplicatedTypeSymbol = db.RepairTypes.Any(a => a.RepairTypeSymbol == model.RepairTypeSymbol);
                }
                else
                {
                    duplicatedTypeSymbol = db.RepairTypes.Any(a => a.RepairTypeSymbol == model.RepairTypeSymbol && a.RepairTypeId != model.RepairTypeId);
                }
                if (duplicatedTypeSymbol == true)
                {
                    ModelState.AddModelError("RepairTypeSymbol", Resource.RepairTypeSymbolAlreadyExist);
                }

                if (model.Notes != null && model.Notes.Length > 500) //độ dài tối đa là 500
                {
                    ModelState.AddModelError("Notes", Resource.RepairTypeNotesTooLong);
                }
            }
        }

        public void AssignRepairTypeValues(RepairType repairType, RepairTypeViewModel model)
        {
            repairType.RepairTypeName = model.RepairTypeName;
            repairType.RepairTypeSymbol = model.RepairTypeSymbol;
            repairType.Notes = model.Notes;
            if (model.RepairTypeId == null)
            {
                repairType.CreatedBy = model.CreatedBy;
                repairType.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                repairType.IsoUtcCreatedOn = util.GetIsoUtcNow();
            }
            else
            {
                repairType.ModifiedBy = model.ModifiedBy;
                repairType.ModifiedOn = util.GetSystemTimeZoneDateTimeNow();
                repairType.IsoUtcModifiedOn = util.GetIsoUtcNow();
            }
        }

        public async Task<bool> SaveRecord(RepairTypeViewModel model)
        {
            bool result = true;
            if (model != null)
            {
                string repairTypeId = "";
                string type = "";
                try
                {
                    model.CreatedBy = _userManager.GetUserId(User);
                    model.ModifiedBy = _userManager.GetUserId(User);
                    if (model.RepairTypeId != null)
                    {
                        type = "update";
                        RepairType repairType = db.RepairTypes.FirstOrDefault(a => a.RepairTypeId == model.RepairTypeId);
                        AssignRepairTypeValues(repairType, model);
                        db.Entry(repairType).State = EntityState.Modified;
                        db.SaveChanges();
                        repairTypeId = repairType.RepairTypeId;
                    }
                    else
                    {
                        type = "create";
                        RepairType repairType = new RepairType();
                        AssignRepairTypeValues(repairType, model);
                        repairType.RepairTypeId = Guid.NewGuid().ToString();
                        db.RepairTypes.Add(repairType);
                        db.SaveChanges();
                        repairTypeId = repairType.RepairTypeId;
                    }
                }
                catch (Exception ex)
                {
                    if (type == "create")
                    {
                        if (!string.IsNullOrEmpty(repairTypeId))
                        {
                            RepairType repairType = db.RepairTypes.FirstOrDefault(a => a.RepairTypeId == repairTypeId);
                            if (repairType != null)
                            {
                                db.RepairTypes.Remove(repairType);
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

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RepairType, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    RepairType repairType = db.RepairTypes.Where(a => a.RepairTypeId == Id).FirstOrDefault();
                    if (repairType != null)
                    {
                        db.RepairTypes.Remove(repairType);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                RepairType repairType = db.RepairTypes.Where(a => a.RepairTypeId == Id).FirstOrDefault();
                if (repairType == null)
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