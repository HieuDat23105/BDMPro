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
using System.Linq.Expressions;

namespace BDMPro.Controllers
{
    public class SupplierController : Controller
    {
        private DefaultDBContext db;
        private Util util;
        private readonly UserManager<AspNetUsers> _userManager;
        private ErrorLoggingService _logger;
        private IWebHostEnvironment Environment;

        public SupplierController(DefaultDBContext db, Util util, UserManager<AspNetUsers> userManager, ErrorLoggingService logger, IWebHostEnvironment environment)
        {
            this.db = db;
            this.util = util;
            _userManager = userManager;
            _logger = logger;
        }


        public IActionResult Index()
        {
            // ViewData["StatusSelectList"] = util.GetDataForDropDownList("", db.GlobalOptionSets, a => a.DisplayName, a => a.DisplayName, a => a.Type == "UserStatus");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewSupplier(string sort, string search, int? pg, int? size)
        {
            try
            {
                List<ColumnHeader> headers = new List<ColumnHeader>();
                if (string.IsNullOrEmpty(sort))
                {
                    sort = SupplierListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(SupplierListConfig.DefaultColumnHeaders, sort);
                var list = ReadSuppliers();
                string searchMessage = SupplierListConfig.SearchMessage;
                list = SupplierListConfig.PerformSearch(list, search);
                list = SupplierListConfig.PerformSort(list, sort);
                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;
                int? total = list.Count();
                int? defaultSize = SupplierListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;
                PaginatedList<SupplierViewModel> result = await PaginatedList<SupplierViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, searchMessage);
                return PartialView("~/Views/Exam/_MainList.cshtml", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return PartialView("~/Views/Shared/Error.cshtml", null);
        }

        public IQueryable<SupplierViewModel> ReadSuppliers()
        {
            try
            {
                var suppier = from supplier in db.Suppliers
                            select new SupplierViewModel
                            {
                                SupplierId = supplier.SupplierId,
                                SupplierName = supplier.SupplierName,
                                Email = supplier.Email,
                                Phone = supplier.Phone,
                                Address = supplier.Address,
                                // ContactId = supplier.ContactId,
                                // Notes = supplier.Notes,
                                // CreatedBy = supplier.CreatedBy,
                                // CreatedOn = supplier.CreatedOn,
                                // ModifiedBy = supplier.ModifiedBy,
                                // ModifiedOn = supplier.ModifiedOn,
                                // IsoUtcCreatedOn = supplier.IsoUtcCreatedOn,
                                // IsoUtcModifiedOn = supplier.IsoUtcModifiedOn,
                                // IsActive = supplier.IsActive,
                                // IsDeleted = supplier.IsDeleted
                            };
                return query;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return null;
        }
        public SupplierViewModel GetSupplierViewModel(string supplierId, string type)
        {
            SupplierViewModel model = new SupplierViewModel();
            try
            {
                model = (from supplier in db.Suppliers
                         where supplier.SupplierId == supplierId
                         select new SupplierViewModel
                         {
                             SupplierId = supplier.SupplierId,
                             SupplierName = supplier.SupplierName,
                             Email = supplier.Email,
                             Phone = supplier.Phone,
                             Address = supplier.Address,
                            //  ContactId = supplier.ContactId,
                            //  Notes = supplier.Notes,
                            //  CreatedBy = supplier.CreatedBy,
                            //  CreatedOn = supplier.CreatedOn,
                            //  IsoUtcCreatedOn = supplier.IsoUtcCreatedOn,
                            //  ModifiedBy = supplier.ModifiedBy,
                            //  ModifiedOn = supplier.ModifiedOn,
                            //  IsoUtcModifiedOn = supplier.IsoUtcModifiedOn,
                         }).FirstOrDefault();

                if (type == "View")
                {
                    // Lưu ý: Bạn cần thêm hàm GetCreatedAndModified vào class util của bạn để có thể sử dụng nó ở đây
                    // model.CreatedAndModified = util.GetCreatedAndModified(model.CreatedBy, model.IsoUtcCreatedOn, model.ModifiedBy, model.IsoUtcModifiedOn);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        // public void SetupSelectLists(SupplierViewModel model)
        // {
        //     model.SupplierStatusSelectList = util.GetDataForDropDownList(model.SupplierStatusId, db.GlobalOptionSets, a => a.DisplayName, a => a.Id, a => a.Type == "SupplierStatus", a => a.OptionOrder, "asc");
        // }

        public IActionResult Import()
        {
            ImportFromExcel importFromExcel = new ImportFromExcel();
            return View(importFromExcel);
        }

        public IActionResult DownloadImportTemplate()
        {
            var path = Path.Combine(this.Environment.WebRootPath, "Assets", "SupplierExcelTemplate.xlsx");
            var tempFilePath = Path.Combine(this.Environment.WebRootPath, "Assets", "ImportSuppliersFromExcel.xlsx");
            List<string> countries = db.Countries.Select(a => a.Name).ToList();
            byte[] fileBytes = util.CreateDropDownListValueInExcel(path, tempFilePath, countries, "Country");
            if (fileBytes == null)
            {
                return null;
            }
            string dtnow = util.GetIsoUtcNow();
            dtnow = dtnow.Replace("-", "");
            dtnow = dtnow.Replace(":", "");
            dtnow = dtnow.Replace(".", "");
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, $"ImportSuppliersFromExcel{dtnow}.xlsx");
        }

        // [HttpPost]
        // public async Task<IActionResult> Import(ImportFromExcel model, IFormFile File)
        // {
        //     try
        //     {
        //         List<string> errors = new List<string>();
        //         List<ImportFromExcelError> errorsList = new List<ImportFromExcelError>();
        //         var suppliers = new List<Supplier>();

        //         int successCount = 0;
        //         int dtRowsCount = 0;
        //         List<string> columns = new List<string>();
        //         using (var memoryStream = new MemoryStream())
        //         {
        //             File.CopyTo(memoryStream);
        //             using (var reader = ExcelReaderFactory.CreateReader(memoryStream))
        //             {
        //                 var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
        //                 {
        //                     ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
        //                     {
        //                         UseHeaderRow = true
        //                     }
        //                 });

        //                 var dt = ds.Tables[0];

        //                 foreach (var col in dt.Columns.Cast<DataColumn>())
        //                 {
        //                     col.ColumnName = col.ColumnName.Replace("*", "");
        //                     columns.Add(col.ColumnName);
        //                 }
        //                 dtRowsCount = dt.Rows.Count;

        //                 errors = util.ValidateColumns(columns, new List<string>
        //         {
        //             "SupplierId","SupplierName","Email",
        //             "Phone","Address","ContactId",
        //             "Notes",
        //         });

        //                 //if all columns validated
        //                 if (errors.Count == 0)
        //                 {
        //                     for (int i = 0; i < dtRowsCount; i++)
        //                     {
        //                         try
        //                         {
        //                             string supplierId = dt.Rows[i].Field<string>("SupplierId");
        //                             string supplierName = dt.Rows[i].Field<string>("SupplierName");
        //                             string email = dt.Rows[i].Field<string>("Email");
        //                             string phone = dt.Rows[i].Field<string>("Phone");
        //                             string address = dt.Rows[i].Field<string>("Address");
        //                             string contactId = dt.Rows[i].Field<string>("ContactId");
        //                             string notes = dt.Rows[i].Field<string>("Notes");

        //                             var upModel = new SupplierViewModel
        //                             {
        //                                 SupplierId = supplierId,
        //                                 SupplierName = supplierName,
        //                                 Email = email,
        //                                 Phone = phone,
        //                                 Address = address,
        //                                 ContactId = contactId,
        //                                 Notes = notes
        //                             };

        //                             errors = util.ValidateImportSupplierFromExcel(upModel);

        //                             if (errors.Count > 0)
        //                             {
        //                                 ImportFromExcelError importFromExcelError = new ImportFromExcelError();
        //                                 importFromExcelError.Row = $"At Row {i + 2}";
        //                                 importFromExcelError.Errors = errors;
        //                                 errorsList.Add(importFromExcelError);
        //                                 continue;
        //                             }

        //                             // Create supplier and save in db (assume creationResult is declared elsewhere)
        //                             var creationResult = await _supplierManager.CreateAsync(upModel);

        //                             if (creationResult.Succeeded)
        //                             {
        //                                 successCount++;
        //                             }
        //                             ModelState.Clear();
        //                         }
        //                         catch (Exception ex)
        //                         {
        //                             errors.Add($"{ex.Message} - Row: {i + 2}");
        //                             _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
        //                         }
        //                     }
        //                 }
        //                 else
        //                 {
        //                     ImportFromExcelError importFromExcelError = new ImportFromExcelError();
        //                     importFromExcelError.Row = Resource.InvalidUserTemplate;
        //                     importFromExcelError.Errors = errors;
        //                     errorsList.Add(importFromExcelError);
        //                 }
        //             }
        //         }
        //         if (errorsList.Count > 0)
        //         {
        //             model.ErrorList = errorsList;
        //             model.UploadResult = $"{successCount} {Resource.outof} {dtRowsCount} {Resource.recordsuploaded}";
        //             return View("import", model);
        //         }
        //         TempData["NotifySuccess"] = Resource.RecordsImportedSuccessfully;
        //     }
        //     catch (Exception ex)
        //     {
        //         TempData["NotifyFailed"] = Resource.FailedExceptionError;
        //         _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
        //     }
        //     return RedirectToAction("index");
        // }

        public IActionResult ViewRecord(string Id)
        {
            SupplierViewModel model = new SupplierViewModel();
            if (Id != null)
            {
                model = GetSupplierViewModel(Id, "View");
            }
            return View(model);
        }

        public IActionResult Edit(string Id)
        {
            SupplierViewModel model = new SupplierViewModel();
            if (Id != null)
            {
                model = GetSupplierViewModel(Id, "Edit");
            }
            // SetupSelectLists(model);
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(SupplierViewModel model)
        {
            try
            {
                ValidateModel(model);

                if (!ModelState.IsValid)
                {
                    // SetupSelectLists(model);
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

        public void ValidateModel(SupplierViewModel model)
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
                if (string.IsNullOrEmpty(model.Address))
                {
                    ModelState.AddModelError("Address", Resource.AddressRequired);
                }
                // Add more validation rules as needed
            }
        }

        public async Task<bool> SaveRecord(SupplierViewModel model)
        {
            bool result = false;
            try
            {
                if (model != null)
                {
                    Supplier supplier = new Supplier();
                    if (model.SupplierId != null)
                    {
                        // Update existing supplier
                        supplier = db.Suppliers.Where(a => a.SupplierId == model.SupplierId).FirstOrDefault();
                        AssignSupplierValues(supplier, model);
                        supplier.ModifiedBy = _userManager.GetUserName(User);
                        supplier.ModifiedOn = DateTime.UtcNow;
                        supplier.IsoUtcModifiedOn = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                        db.Suppliers.Update(supplier);
                    }
                    else
                    {
                        // Create new supplier
                        AssignSupplierValues(supplier, model);
                        supplier.SupplierId = Guid.NewGuid().ToString();
                        supplier.CreatedBy = _userManager.GetUserName(User);
                        supplier.CreatedOn = DateTime.UtcNow;
                        supplier.IsoUtcCreatedOn = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                        db.Suppliers.Add(supplier);
                    }
                    await db.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return result;
        }

        private void AssignSupplierValues(Supplier supplier, SupplierViewModel model)
        {
            supplier.SupplierName = model.SupplierName;
            supplier.Email = model.Email;
            supplier.Phone = model.Phone;
            supplier.Address = model.Address;
            // supplier.ContactId = model.ContactId;
            // supplier.Notes = model.Notes;
        }
        //18/03/2024

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