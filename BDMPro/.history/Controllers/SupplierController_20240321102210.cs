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

        [HttpPost]
        public async Task<IActionResult> Edit(SupplierViewModel model)
        {
            try
            {
                ValidateSupplierModel(model);

                bool result = await SaveSupplierRecord(model);
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
            return RedirectToAction("index", "user");
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

        public void AssignSupplierValues(Exam exam, SupplierViewModel model)
        {
            exam.SupplierName = model.SupplierName;
            exam.StartDate = util.ConvertToSystemTimeZoneDateTime(model.StartDateIsoUtc);
            exam.EndDate = util.ConvertToSystemTimeZoneDateTime(model.EndDateIsoUtc);
            exam.Duration = model.Duration;
            exam.MarksToPass = model.MarksToPass;
            exam.RandomizeQuestions = model.RandomizeQuestions;
            exam.IsActive = model.IsActive == "Active" ? true : false;
            exam.ReleaseAnswer = model.ReleaseAnswer;
        }

        public async Task<bool> SaveSupplierRecord(SupplierViewModel model)
        {
            bool result = true;
            if (model != null)
            {
                string supplierId = "";
                string type = "";
                try
                {
                    model.CreatedBy = _userManager.GetUserId(User);
                    model.ModifiedBy = _userManager.GetUserId(User);
                    //chỉnh sửa
                    if (model.SupplierId != null)
                    {
                        type = "update";
                        //lưu thông tin nhà cung cấp
                        Supplier supplier = _db.Suppliers.FirstOrDefault(a => a.SupplierId == model.SupplierId);
                        AssignSupplierValues(supplier, model);
                        _db.Entry(supplier).State = EntityState.Modified;

                        //lưu vào Supplier
                        _db.SaveChanges();

                        supplierId = supplier.SupplierId;
                    }
                    //Đăng ký bản ghi nhà cung cấp mới
                    else
                    {
                        type = "create";
                        //Luu thông tin nhà cung cấp
                        Supplier supplier = new Supplier();
                        AssignSupplierValues(supplier, model);
                        supplier.SupplierId = Guid.NewGuid().ToString();
                        _db.Suppliers.Add(supplier);
                        _db.SaveChanges();
                        supplierId = supplier.SupplierId;
                    }
                }
                catch (Exception ex)
                {
                    //Ngoại lệ khi tạo bản ghi mới, có nghĩa là bản ghi tạo không đầy đủ do lỗi, hoàn tác việc tạo bản ghi
                    if (type == "create")
                    {
                        if (!string.IsNullOrEmpty(supplierId))
                        {
                            Supplier supplier = _db.Suppliers.FirstOrDefault(a => a.SupplierId == supplierId);
                            if (supplier != null)
                            {
                                _db.Suppliers.Remove(supplier);
                                _db.SaveChanges();
                            }
                        }
                    }
                    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                    return false;
                }
            }
            return result;
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