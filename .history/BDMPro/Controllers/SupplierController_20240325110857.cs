using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using BDMPro.Models;
using System.Data;
using System.Globalization;
using BDMPro.Resources;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BDMPro.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using BDMPro.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using BDMPro.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BDMPro.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;
        private readonly IConfiguration _configuration;
        private DefaultDBContext db;
        private Util util;
        private ErrorLoggingService _logger;
        private IWebHostEnvironment Environment;

        public SupplierController(DefaultDBContext _db, UserManager<AspNetUsers> userManager,
                              SignInManager<AspNetUsers> signInManager, Util _util, ErrorLoggingService logger, IConfiguration configuration, IWebHostEnvironment _environment)
        {
            db = _db;
            _userManager = userManager;
            _signInManager = signInManager;
            util = _util;
            _logger = logger;
            _configuration = configuration;
            Environment = _environment;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "true", "", "", "")]
        public IActionResult Index()
        {
            ViewData["StatusSelectList"] = util.GetDataForDropDownList("", db.GlobalOptionSets, a => a.DisplayName, a => a.DisplayName, a => a.Type == "SupplierStatus");
            ViewData["ContactSelectList"] = util.GetDataForDropDownList("", db.Contacts, a => a.ContactName, a => a.ContactName);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewSupplier([FromBody] dynamic requestData)
        {
            string sort = requestData.sort?.Value;
            int? size = (int.TryParse(requestData.size.Value, out int parsedSize)) ? parsedSize : null;
            string search = requestData.search?.Value;
            int? pg = (int.TryParse(requestData.pg.Value, out int parsedPg)) ? parsedPg : 1;
            string status = requestData.status?.Value;

            try
            {
                List<ColumnHeader> headers = new List<ColumnHeader>();
                if (string.IsNullOrEmpty(sort))
                {
                    sort = SupplierListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(SupplierListConfig.DefaultColumnHeaders, sort);
                var list = ReadSupplierList();
                string searchMessage = SupplierListConfig.SearchMessage;
                list = SupplierListConfig.PerformSearch(list, search);
                list = SupplierListConfig.PerformSearch(list, status);
                list = SupplierListConfig.PerformSort(list, sort);
                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;
                int? total = list.Count();
                int? defaultSize = SupplierListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;
                PaginatedList<SupplierViewModel> result = await PaginatedList<SupplierViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, searchMessage);
                return PartialView("~/Views/Supplier/_MainList.cshtml", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return PartialView("~/Views/Shared/Error.cshtml", null);
        }

        public IQueryable<SupplierViewModel> ReadSupplierList()
        {
            var supplierList = from t1 in db.Suppliers.AsNoTracking()
                               let t3 = db.GlobalOptionSets.FirstOrDefault(g => g.Id == t1.SupplierStatusId)
                               where t1.IsDeleted == false
                               select new SupplierViewModel
                               {
                                   SupplierId = t1.SupplierId,
                                   SupplierName = t1.SupplierName,
                                   EmailAddress = t1.Email,
                                   PhoneNumber = t1.PhoneNumber,
                                   Address = t1.Address,
                                   CreatedOn = t1.CreatedOn,
                                   IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                                   SupplierStatusName = t3 != null ? t3.DisplayName : "",
                                   SupplierContactNameList = (from t4 in db.SupplierContacts
                                                       join t5 in db.Contacts on t4.ContactId equals t5.ContactId
                                                       where t4.SupplierId == t1.SupplierId
                                                       orderby t5.ContactName
                                                       select t5.ContactName).Distinct().ToList()
                               };

            return supplierList;
        }


        public SupplierViewModel GetSupplierViewModel(string Id, string type)
        {
            SupplierViewModel model = new SupplierViewModel();
            try
            {
                model = (from t1 in db.Suppliers
                         let t2 = db.Contacts.FirstOrDefault(c => c.ContactId == t1.ContactId)
                         where t1.SupplierId == Id
                         select new SupplierViewModel
                         {
                             SupplierId = t1.SupplierId,
                             EmailAddress = t1.Email,
                             PhoneNumber = t1.PhoneNumber,
                             Address = t1.Address,
                             SupplierName = t1.SupplierName,
                             Notes = t1.Notes,
                             SupplierStatusId = t1.SupplierStatusId,
                             CreatedBy = t1.CreatedBy,
                             ModifiedBy = t1.ModifiedBy,
                             CreatedOn = t1.CreatedOn,
                             ModifiedOn = t1.ModifiedOn,
                             IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                             IsoUtcModifiedOn = t1.IsoUtcModifiedOn,
                         }).FirstOrDefault();
                model.SupplierStatusName = db.GlobalOptionSets.Where(a => a.Id == model.SupplierStatusId).Select(a => a.DisplayName).FirstOrDefault();
                model.SupplierIdList = (from t1 in db.AspNetUserRoles
                                        join t2 in db.AspNetRoles on t1.RoleId equals t2.Id
                                        where t1.UserId == model.AspNetUserId
                                        select t2.Name).ToList();
                model.UserRoleName = String.Join(", ", model.UserRoleIdList);
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

        public void SetupSupplierSelectLists(SupplierViewModel model)
        {
            model.SupplierStatusSelectList = util.GetDataForDropDownList(model.SupplierStatusId, db.GlobalOptionSets, a => a.DisplayName, a => a.Id, a => a.Type == "SupplierStatus", a => a.OptionOrder, "asc");
            model.ContactSelectList = util.GetContactsForMultiSelect(model.ContactIdList);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "", "true", "true", "")]
        public IActionResult Import()
        {
            ImportFromExcel importFromExcel = new ImportFromExcel();
            return View(importFromExcel);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "true", "true", "true", "")]
        public IActionResult DownloadImportTemplate()
        {
            var path = Path.Combine(this.Environment.WebRootPath, "Assets", "SupplierExcelTemplate.xlsx");
            var tempFilePath = Path.Combine(this.Environment.WebRootPath, "Assets", "ImportSuppliersFromExcel.xlsx");
            List<string> contacts = db.Contacts.Select(a => a.ContactName).ToList();
            byte[] fileBytes = util.CreateDropDownListValueInExcel(path, tempFilePath, contacts, "Contact");
            if (fileBytes == null)
            {
                return null;
            }
            string dtnow = util.GetIsoUtcNow();
            dtnow = dtnow.Replace("-", "");
            dtnow = dtnow.Replace(":", "");
            dtnow = dtnow.Replace(".", "");
            return File(System.Net.Mime.MediaTypeNames.Application.Octet, $"ImportSuppliersFromExcel{dtnow}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> Import(ImportFromExcel model, IFormFile File)
        {
            try
            {
                List<string> errors = new List<string>();
                List<ImportFromExcelError> errorsList = new List<ImportFromExcelError>();
                var suppliers = new List<Supplier>();

                SupplierViewModel upModel = new SupplierViewModel();

                int successCount = 0;
                int dtRowsCount = 0;
                List<string> columns = new List<string>();
                using (var memoryStream = new MemoryStream())
                {
                    File.CopyTo(memoryStream);
                    using (var reader = ExcelReaderFactory.CreateReader(memoryStream))
                    {
                        var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        var dt = ds.Tables[0];

                        foreach (var col in dt.Columns.Cast<DataColumn>())
                        {
                            col.ColumnName = col.ColumnName.Replace("*", "");
                            columns.Add(col.ColumnName);
                        }
                        dtRowsCount = dt.Rows.Count;

                        errors = util.ValidateColumns(columns, new List<string>
                        {
                            "Supplier Name","Email Address Address","PhoneNumber","Address","Contact",
                        });

                        if (errors.Count == 0)
                        {
                            for (int i = 0; i < dtRowsCount; i++)
                            {
                                try
                                {
                                    string supplierName = dt.Rows[i].Field<string>("Supplier Name");
                                    string email = dt.Rows[i].Field<string>("Email Address Address");
                                    object phoneNumberObject = dt.Rows[i]["Phone Number"];
                                    string phone = Convert.ToString(phoneNumberObject);
                                    string address = dt.Rows[i].Field<string>("Address");
                                    string contactId = dt.Rows[i].Field<string>("Contact");

                                    upModel.SupplierName = supplierName;
                                    upModel.EmailAddress = email;
                                    upModel.ContactId = contactId;
                                    upModel.Address = address;
                                    upModel.PhoneNumber = phone;

                                    errors = util.ValidateImportSupplierFromExcel(upModel);

                                    if (errors.Count() > 0)
                                    {
                                        ImportFromExcelError importFromExcelError = new ImportFromExcelError();
                                        importFromExcelError.Row = $"At Row {i + 2}";
                                        importFromExcelError.Errors = errors;
                                        errorsList.Add(importFromExcelError);
                                        continue;
                                    }
                                    Supplier supplier = new Supplier();
                                    supplier.SupplierId = Guid.NewGuid().ToString();
                                    supplier.SupplierName = supplierName;
                                    supplier.Email = email;
                                    supplier.PhoneNumber = phone;
                                    supplier.Address = address;
                                    supplier.ContactId = contactId;
                                    supplier.SupplierStatusId = util.GetGlobalOptionSetId(ProjectEnum.SupplierStatus.Active.ToString(), "SupplierStatus");
                                    supplier.CreatedBy = _userManager.GetUserId(User);
                                    supplier.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                                    supplier.IsoUtcCreatedOn = util.GetIsoUtcNow();
                                    db.Suppliers.Add(supplier);
                                    db.SaveChanges();
                                    successCount++;
                                    ModelState.Clear();
                                }
                                catch (Exception ex)
                                {
                                    errors.Add($"{ex.Message} - Row: {i + 2}");
                                    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                                }
                            }
                        }
                        else
                        {
                            ImportFromExcelError importFromExcelError = new ImportFromExcelError();
                            importFromExcelError.Row = Resource.InvalidSupplierTemplate;
                            importFromExcelError.Errors = errors;
                            errorsList.Add(importFromExcelError);
                        }
                    }
                }
                if (errorsList.Count > 0)
                {
                    model.ErrorList = errorsList;
                    model.UploadResult = $"{successCount} {Resource.outof} {dtRowsCount} {Resource.recordsuploaded}";
                    return View("import", model);
                }
                TempData["NotifySuccess"] = Resource.RecordsImportedSuccessfully;
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index");
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "", "true", "true", "")]
        public IActionResult Edit(string Id)
        {
            SupplierViewModel model = new SupplierViewModel();
            if (Id != null)
            {
                model = GetSupplierViewModel(Id, "Edit");
            }
            SetupSupplierSelectLists(model);
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "true", "", "", "")]
        public IActionResult ViewRecord(string Id)
        {
            SupplierViewModel model = new SupplierViewModel();
            if (Id != null)
            {
                model = GetSupplierViewModel(Id, "View");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SupplierViewModel model)
        {
            try
            {
                ValidateSupplierModel(model);

                if (!ModelState.IsValid)
                {
                    SetupSupplierSelectLists(model);
                    return View(model);
                }

                bool result = await SaveRecord(model);
                if (result == false)
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
                else
                {
                    ModelState.Clear();
                    TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
                }
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index", "supplier");
        }

        public void ValidateSupplierModel(SupplierViewModel model)
        {
            if (model != null)
            {
                bool suppliernameExist = util.SupplierNameExists(model.SupplierName, model.SupplierId);
                bool emailExist = util.EmailExists(model.EmailAddress, model.SupplierId);
                if (suppliernameExist)
                {
                    ModelState.AddModelError("SupplierName", Resource.SupplierNameTaken);
                }
                if (emailExist)
                {
                    ModelState.AddModelError("Email Address", Resource.EmailAddressTaken);
                }
                if (string.IsNullOrEmpty(model.SupplierName))
                {
                    ModelState.AddModelError("SupplierName", Resource.SupplierNameRequired);
                }
                // if (string.IsNullOrEmpty(model.ContactId))
                // {
                //     ModelState.AddModelError("ContactId", Resource.ContactIdRequired);
                // }
                if (string.IsNullOrEmpty(model.Address))
                {
                    ModelState.AddModelError("Address", Resource.AddressRequired);
                }
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", Resource.PhoneRequired);
                }
                if (model.SupplierStatusId == null || model.SupplierStatusId == "null")
                {
                    ModelState.AddModelError("SupplierStatusId", Resource.StatusRequired);
                }
            }
        }

        public void AssignSupplierValues(Supplier supplier, SupplierViewModel model)
        {
            supplier.SupplierName = model.SupplierName;
            supplier.PhoneNumber = model.PhoneNumber;
            supplier.Address = model.Address;
            supplier.ContactId = model.ContactId;
            supplier.Email = model.EmailAddress;
            supplier.Notes = model.Notes;
            supplier.SupplierStatusId = string.IsNullOrEmpty(model.SupplierStatusId) ? util.GetGlobalOptionSetId(SupplierStatus.Active.ToString(), "SupplierStatus") : model.SupplierStatusId;
            if (model.SupplierId == null)
            {
                supplier.CreatedBy = model.CreatedBy;
                supplier.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                supplier.IsoUtcCreatedOn = util.GetIsoUtcNow();
            }
            else
            {
                supplier.ModifiedBy = model.ModifiedBy;
                supplier.ModifiedOn = util.GetSystemTimeZoneDateTimeNow();
                supplier.IsoUtcModifiedOn = util.GetIsoUtcNow();
            }
        }

        public async Task<bool> SaveRecord(SupplierViewModel model)
        {
            bool result = true;
            if (model != null)
            {
                try
                {
                    if (model.SupplierId != null)
                    {
                        // Update existing supplier
                        Supplier supplier = db.Suppliers.FirstOrDefault(a => a.SupplierId == model.SupplierId);
                        if (supplier != null)
                        {
                            AssignSupplierValues(supplier, model);
                            db.Entry(supplier).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        // Create new supplier
                        Supplier supplier = new Supplier();
                        AssignSupplierValues(supplier, model);
                        supplier.SupplierId = Guid.NewGuid().ToString();
                        db.Suppliers.Add(supplier);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    Supplier supplier = db.Suppliers.Where(a => a.SupplierId == Id).FirstOrDefault();
                    if (supplier != null)
                    {
                        supplier.IsDeleted = true;

                        var inactiveStatusId = db.GlobalOptionSets
                                                 .Where(a => a.Type == "SupplierStatus" && a.Code == "Inactive")
                                                 .Select(a => a.Id)
                                                 .FirstOrDefault();

                        supplier.SupplierStatusId = inactiveStatusId;

                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                Supplier supplier = db.Suppliers.Where(a => a.SupplierId == Id).FirstOrDefault();
                if (supplier == null)
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