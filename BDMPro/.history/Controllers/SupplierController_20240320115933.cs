using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BDMPro.Models;
using BDMPro.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BDMPro.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BDMPro.Services;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace BDMPro.Controllers
{
    [Authorize]
    public class SupplierController : Microsoft.AspNetCore.Mvc.Controller
    {
        private DefaultDBContext db;
        private Util util;
        private ErrorLoggingService _logger;
        private readonly UserManager<AspNetUsers> _userManager;

        public SupplierController(DefaultDBContext db, Util util, ErrorLoggingService logger, UserManager<AspNetUsers> userManager)
        {
            this.db = db;
            this.util = util;
            _logger = logger;
            _userManager = userManager;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.SupplierManagement, "true", "", "", "")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewLoginHistories([FromBody] dynamic requestData)
        {
            try
            {
                string sort = requestData.sort?.Value;
                int? size = (int.TryParse(requestData.size.Value, out int parsedSize)) ? parsedSize : null;
                string search = requestData.search?.Value;
                int? pg = (int.TryParse(requestData.pg.Value, out int parsedPg)) ? parsedPg : 1;
                string datevalue = requestData.datevalue?.Value;
                string offsetString = requestData.offset?.Value.ToString();
                double offset = (double.TryParse(offsetString, out double parsedOffset)) ? parsedOffset : 0;

                List<ColumnHeader> headers = new List<ColumnHeader>();
                if (string.IsNullOrEmpty(sort))
                {
                    sort = SupplierListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(SupplierListConfig.DefaultColumnHeaders, sort);
                var list = ReadLoginHistories();
                string searchMessage = SupplierListConfig.SearchMessage;
                list = SupplierListConfig.PerformSearch(list, search);
                list = SupplierListConfig.PerformSearchDateTime(list, datevalue, offset);
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

        public IQueryable<SupplierViewModel> ReadLoginHistories()
        {
            var SupplierList = from t1 in db.Suppliers
                       select new SupplierViewModel
                       {
                           SupplierId = t1.SupplierId,
                           SupplierName = t1.SupplierName,
                       };
            return SupplierList;
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