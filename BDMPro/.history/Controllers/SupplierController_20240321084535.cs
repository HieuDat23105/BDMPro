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
                // string sort = requestData.sort?.Value;
                // int? size = (int.TryParse(requestData.size.Value, out int parsedSize)) ? parsedSize : null;
                // string search = requestData.search?.Value;
                // int? pg = (int.TryParse(requestData.pg.Value, out int parsedPg)) ? parsedPg : 1;

                List<ColumnHeader> headers = new List<ColumnHeader>();
                // if (string.IsNullOrEmpty(sort))
                // {
                //     sort = SupplierListConfig.DefaultSortOrder;
                // }
                // headers = ListUtil.GetColumnHeaders(SupplierListConfig.DefaultColumnHeaders, sort);

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
                                       CreatedOn = t1.CreatedOn.ToString()
                                   };
                return supplierList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return null;
        }


    }
}