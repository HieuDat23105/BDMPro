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
                           let t3 = db.GlobalOptionSets.Where(g => g.Id == t1.SupplierStatusId).SingleOrDefault()
                               where t1.IsDeleted == false
                               select new SupplierViewModel
                               {
                                   SupplierId = t1.SupplierId,
                                   SupplierName = t1.SupplierName,
                                   Email = t1.Email,
                                   PhoneNumber = t1.PhoneNumber,
                                   Address = t1.Address,
                                   ContactId = t1.ContactId,
                                   CreatedOn = t1.CreatedOn,
                                   IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                                   SupplierStatusName = t3 == null ? "" : t3.DisplayName
                               };

            return supplierList;
        }

public SupplierViewModel GetSupplierViewModel(string Id, string type)
{
    UserProfileViewModel model = new UserProfileViewModel();
            try
            {
                string profilePicTypeId = util.GetGlobalOptionSetId(ProjectEnum.UserAttachment.ProfilePicture.ToString(), "UserAttachment");
                model = (from t1 in db.UserProfiles
                         join t2 in db.AspNetUsers on t1.AspNetUserId equals t2.Id
                         where t1.Id == Id
                         select new UserProfileViewModel
                         {
                             Id = t1.Id,
                             AspNetUserId = t1.AspNetUserId,
                             FullName = t1.FullName,
                             IDCardNumber = t1.IDCardNumber,
                             FirstName = t1.FirstName,
                             LastName = t1.LastName,
                             DateOfBirth = t1.DateOfBirth,
                             PhoneNumber = t1.PhoneNumber,
                             Address = t1.Address,
                             PostalCode = t1.PostalCode,
                             Username = t2.UserName,
                             EmailAddress = t2.Email,
                             UserStatusId = t1.UserStatusId,
                             GenderId = t1.GenderId,
                             CountryName = t1.CountryName,
                             CreatedBy = t1.CreatedBy,
                             ModifiedBy = t1.ModifiedBy,
                             CreatedOn = t1.CreatedOn,
                             ModifiedOn = t1.ModifiedOn,
                             IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                             IsoUtcModifiedOn = t1.IsoUtcModifiedOn,
                             IsoUtcDateOfBirth = t1.IsoUtcDateOfBirth
                         }).FirstOrDefault();
                model.UserStatusName = db.GlobalOptionSets.Where(a => a.Id == model.UserStatusId).Select(a => a.DisplayName).FirstOrDefault();
                model.UserRoleIdList = (from t1 in db.AspNetUserRoles
                                        join t2 in db.AspNetRoles on t1.RoleId equals t2.Id
                                        where t1.UserId == model.AspNetUserId
                                        select t2.Name).ToList();
                model.UserRoleName = String.Join(", ", model.UserRoleIdList);
                model.GenderName = db.GlobalOptionSets.Where(a => a.Id == model.GenderId).Select(a => a.DisplayName).FirstOrDefault();
                model.ProfilePictureFileName = db.UserAttachments.Where(a => a.UserProfileId == model.Id && a.AttachmentTypeId == profilePicTypeId).OrderByDescending(a => a.CreatedOn).Select(a => a.UniqueFileName).FirstOrDefault();
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

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "true", "true", "")]
        public IActionResult Import()
        {
            ImportFromExcel importFromExcel = new ImportFromExcel();
            return View(importFromExcel);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "true", "true", "true", "")]
        public IActionResult DownloadSupplierImportTemplate()
        {
            var path = Path.Combine(this.Environment.WebRootPath, "Assets", "SupplierExcelTemplate.xlsx");
            var tempFilePath = Path.Combine(this.Environment.WebRootPath, "Assets", "ImportSuppliersFromExcel.xlsx");
            string dtnow = util.GetIsoUtcNow();
            dtnow = dtnow.Replace("-", "");
            dtnow = dtnow.Replace(":", "");
            dtnow = dtnow.Replace(".", "");
            return File(System.Net.Mime.MediaTypeNames.Application.Octet, $"ImportSuppliersFromExcel{dtnow}.xlsx");
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

                        errors = util.ValidateColumns(columns, new List<string>
                {
                    "Supplier Name","Email","Contact Id",
                    "Address","PhoneNumber",
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
                                    string phone = dt.Rows[i].Field<string>("PhoneNumber");

                                    supplierModel.SupplierName = supplierName;
                                    supplierModel.Email = email;
                                    supplierModel.ContactId = contactId;
                                    supplierModel.Address = address;
                                    supplierModel.PhoneNumber = phone;

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
                                    supplier.PhoneNumber = supplierModel.PhoneNumber;
                                    db.Suppliers.Add(supplier);
                                    db.SaveChanges();

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
            model.SupplierSelectList = util.GetDataForDropDownList(model.SupplierStatusId, db.GlobalOptionSets, a => a.DisplayName, a => a.Id, a => a.Type == "SupplierStatus", a => a.OptionOrder, "asc");
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
                bool emailExist = util.EmailExists(model.Email, model.SupplierId);
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
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", Resource.PhoneRequired);
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
            supplier.SupplierStatusId = string.IsNullOrEmpty(model.SupplierStatusId) ? util.GetGlobalOptionSetId(SupplierStatus.Active.ToString(), "SupplierStatus") : model.SupplierStatusId;
            supplier.IsActive = model.IsActive == "Active" ? true : false;
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
            if (model == null)
                return false;

            try
            {
                // Kiểm tra xem model có ID không
                if (!string.IsNullOrEmpty(model.SupplierId))
                {
                    // Cập nhật thông tin nhà cung cấp đã tồn tại
                    Supplier existingSupplier = await db.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == model.SupplierId);
                    if (existingSupplier != null)
                    {
                        AssignSupplierValues(existingSupplier, model);
                        db.Entry(existingSupplier).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    // Tạo mới nhà cung cấp
                    Supplier newSupplier = new Supplier();
                    AssignSupplierValues(newSupplier, model);
                    db.Suppliers.Add(newSupplier);
                    await db.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                // Ở đây bạn có thể ghi nhật ký, hoặc thực hiện rollback
                return false;
            }
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    Supplier supplier = db.Suppliers.Where(a => a.SupplierId == Id).FirstOrDefault();
                    if (supplier != null)
                    {
                        bool.TryParse(_configuration["ShowDemoAccount"], out bool showDemoAccount);
                        if (showDemoAccount && (_userManager.GetUserName(User) == "uadmin" || _userManager.GetUserName(User) == "sadmin"))
                        {
                            TempData["NotifyFailed"] = Resource.DemoAccountCannotBeDeleted;
                            return RedirectToAction("index");
                        }
                        db.Suppliers.Remove(supplier);
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