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
                var suppierList = from supplier in db.Suppliers
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
                return suppierList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return null;
        }
    }
}