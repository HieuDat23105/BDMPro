using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using BDMPro.Models;
using System.Data;
using System.Globalization;
using BDMPro.Resources;
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
        private readonly DefaultDBContext _db;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;
        private readonly Util _util;
        private readonly ErrorLoggingService _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public SupplierController(DefaultDBContext db, UserManager<AspNetUsers> userManager,
                                  SignInManager<AspNetUsers> signInManager, Util util, ErrorLoggingService logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _util = util;
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "true", "", "", "")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewSupplier([FromBody] dynamic requestData)
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
                    sort = SupplierListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(SupplierListConfig.DefaultColumnHeaders, sort);

                var list = ReadSupplierList();

                list = SupplierListConfig.PerformSearch(list, search);
                list = SupplierListConfig.PerformSort(list, sort);

                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;

                int? total = await list.CountAsync();
                int? defaultSize = SupplierListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;

                PaginatedList<SupplierViewModel> result = await PaginatedList<SupplierViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, SupplierListConfig.SearchMessage);
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
            try
            {
                var supplierList = from t1 in _db.Suppliers.AsNoTracking()
                                   select new SupplierViewModel
                                   {
                                       SupplierId = t1.SupplierId,
                                       SupplierName = t1.SupplierName,
                                       Email = t1.Email,
                                       Phone = t1.Phone,
                                       Address = t1.Address,
                                       ContactId = t1.ContactId,
                                       CreatedOn = t1.CreatedOn,
                                   };
                return supplierList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return null;
        }
        public SupplierViewModel GetSupplierViewModel(string Id, string type)
        {
            SupplierViewModel model = new SupplierViewModel();
            try
            {
                model = (from t1 in _db.Suppliers
                         where t1.SupplierId == Id
                         select new SupplierViewModel
                         {
                             SupplierId = t1.SupplierId,
                             SupplierName = t1.SupplierName,
                             Email = t1.Email,
                             ContactId = t1.ContactId,
                             Address = t1.Address,
                             Phone = t1.Phone,
                             CreatedOn = t1.CreatedOn,
                         }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "true", "true", "")]
        public IActionResult Import()
        {
            ImportFromExcel importFromExcel = new ImportFromExcel();
            return View(importFromExcel);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "true", "true", "true", "")]
        public IActionResult DownloadSupplierImportTemplate()
        {
            var path = Path.Combine(this._environment.WebRootPath, "Assets", "SupplierExcelTemplate.xlsx");
            var tempFilePath = Path.Combine(this._environment.WebRootPath, "Assets", "ImportSuppliersFromExcel.xlsx");
            List<string> countries = _db.Countries.Select(a => a.Name).ToList();
            byte[] fileBytes = _util.CreateDropDownListValueInExcel(path, tempFilePath, countries, "Country");
            if (fileBytes == null)
            {
                return null;
            }
            string dtnow = _util.GetIsoUtcNow();
            dtnow = dtnow.Replace("-", "");
            dtnow = dtnow.Replace(":", "");
            dtnow = dtnow.Replace(".", "");
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, $"ImportSuppliersFromExcel{dtnow}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> ImportSuppliers(ImportFromExcel model, IFormFile File)
        {
            try
            {
                List<string> errors = new List<string>();
                List<ImportFromExcelError> errorsList = new List<ImportFromExcelError>();

                SupplierViewModel supplierModel = new SupplierViewModel();

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

                        errors = _util.ValidateColumns(columns, new List<string>
                {
                    "Supplier Name","Email","Contact Id",
                    "Address","Phone",
                });

                        // If all columns are valid
                        if (errors.Count == 0)
                        {
                            for (int i = 0; i < dtRowsCount; i++)
                            {
                                try
                                {
                                    string supplierName = dt.Rows[i].Field<string>("Supplier Name");
                                    string email = dt.Rows[i].Field<string>("Email");
                                    string contactId = dt.Rows[i].Field<string>("Contact Id");
                                    string address = dt.Rows[i].Field<string>("Address");
                                    string phone = dt.Rows[i].Field<string>("Phone");

                                    supplierModel.SupplierName = supplierName;
                                    supplierModel.Email = email;
                                    supplierModel.ContactId = contactId;
                                    supplierModel.Address = address;
                                    supplierModel.Phone = phone;

                                    // errors = util.ValidateImportSupplierFromExcel(supplierModel);

                                    if (errors.Count() > 0)
                                    {
                                        ImportFromExcelError importFromExcelError = new ImportFromExcelError();
                                        importFromExcelError.Row = $"At Row {i + 2}";
                                        importFromExcelError.Errors = errors;
                                        errorsList.Add(importFromExcelError);
                                        continue;
                                    }

                                    // After finishing assigning values to supplierModel, create the supplier here
                                    Supplier supplier = new Supplier();
                                    supplier.SupplierId = Guid.NewGuid().ToString();

                                    // Write other things like supplier name, email, etc...
                                    supplier.SupplierName = supplierModel.SupplierName;
                                    supplier.Email = supplierModel.Email;
                                    supplier.ContactId = supplierModel.ContactId;
                                    supplier.Address = supplierModel.Address;
                                    supplier.Phone = supplierModel.Phone;
                                    _db.Suppliers.Add(supplier);
                                    _db.SaveChanges();

                                    successCount++;
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

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "true", "true", "")]
        public IActionResult Edit(string Id)
        {
            SupplierViewModel model = new SupplierViewModel();
            if (Id != null)
            {
                model = GetSupplierViewModel(Id, "Edit");
            }
            return View(model);
        }
        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "true", "", "", "")]
        public IActionResult ViewRecord(string Id)
        {
            SupplierViewModel model = new SupplierViewModel();
            if (Id != null)
            {
                model = GetSupplierViewModel(Id, "View");
            }
            return View(model);
        }

        public void SetupSelectLists(SupplierViewModel model)
        {
            model.SupplierSelectList = _util.GetDataForDropDownList(model.SupplierStatusId, _db.GlobalOptionSets, a => a.DisplayName, a => a.Id, a => a.Type == "SupplierStatus", a => a.OptionOrder, "asc");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SupplierViewModel model)
        {
            try
            {
                ValidateSupplierModel(model);

                if (!ModelState.IsValid)
                {
                    SetupSelectLists(model);
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
                bool emailExist = _util.EmailExists(model.Email, model.SupplierId);
                if (emailExist)
                {
                    ModelState.AddModelError("Email", Resource.EmailAddressTaken);
                }
                if (string.IsNullOrEmpty(model.SupplierName))
                {
                    ModelState.AddModelError("SupplierName", Resource.SupplierNameRequired);
                }
                if (string.IsNullOrEmpty(model.ContactId))
                {
                    ModelState.AddModelError("ContactId", Resource.ContactIdRequired);
                }
                if (string.IsNullOrEmpty(model.Address))
                {
                    ModelState.AddModelError("Address", Resource.AddressRequired);
                }
                if (string.IsNullOrEmpty(model.Phone))
                {
                    ModelState.AddModelError("Phone", Resource.PhoneRequired);
                }
            }
        }

        public void AssignSupplierValues(Supplier supplier, SupplierViewModel model)
        {
            supplier.SupplierName = model.SupplierName;
            supplier.PhoneNumber = model.PhoneNumber;
            supplier.Address = model.Address;
            supplier.ContactId = model.ContactId;
            supplier.Email = model.Email;
            supplier.Notes = model.Notes;
            supplier.SupplierStatusId = string.IsNullOrEmpty(model.SupplierStatusId) ? _util.GetGlobalOptionSetId(SupplierStatus.Active.ToString(), "SupplierStatus") : model.SupplierStatusId;
            supplier.IsActive = model.IsActive == "Active" ? true : false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSupplierRecord(SupplierViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Supplier supplier;
                    if (string.IsNullOrEmpty(model.SupplierId))
                    {
                        // This is a new record
                        supplier = new Supplier
                        {
                            SupplierId = Guid.NewGuid().ToString(),
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = _userManager.GetUserId(User),
                            IsActive = true,
                            IsDeleted = false
                        };
                    }
                    else
                    {
                        // This is an update to an existing record
                        supplier = _db.Suppliers.FirstOrDefault(s => s.SupplierId == model.SupplierId);
                        if (supplier == null)
                        {
                            return NotFound();
                        }
                        supplier.ModifiedOn = DateTime.UtcNow;
                        supplier.ModifiedBy = _userManager.GetUserId(User);
                    }

                    // Update the rest of the fields
                    supplier.SupplierName = model.SupplierName;
                    supplier.Email = model.Email;
                    supplier.Phone = model.Phone;
                    supplier.Address = model.Address;
                    supplier.ContactId = model.ContactId;

                    // Save the record
                    if (string.IsNullOrEmpty(model.SupplierId))
                    {
                        _db.Suppliers.Add(supplier);
                    }
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Model state is invalid, return the model to the view for correction
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                return View("Error");
            }
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    Supplier supplier = _db.Suppliers.Where(a => a.SupplierId == Id).FirstOrDefault();
                    if (supplier != null)
                    {
                        bool.TryParse(_configuration["ShowDemoAccount"], out bool showDemoAccount);
                        if (showDemoAccount && (_userManager.GetUserName(User) == "uadmin" || _userManager.GetUserName(User) == "sadmin"))
                        {
                            TempData["NotifyFailed"] = Resource.DemoAccountCannotBeDeleted;
                            return RedirectToAction("index");
                        }
                        _db.Suppliers.Remove(supplier);
                        _db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                Supplier supplier = _db.Suppliers.Where(a => a.SupplierId == Id).FirstOrDefault();
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
    }
}