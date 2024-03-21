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
       var list = await Task.Run(() => ReadSupplierList().Select(s => new Supplier {
    SupplierName = s.SupplierName,
    Email = s.Email,
    Phone = s.Phone,
    Address = s.Address,
    ContactId = s.ContactId,
    // Chuyển đổi các thuộc tính khác nếu cần
}));


        // Thực hiện tìm kiếm và sắp xếp theo cấu hình SupplierListConfig
        list = SupplierListConfig.PerformSearch(list, search);
        list = SupplierListConfig.PerformSort(list, sort);

        ViewData["CurrentSort"] = sort;
        ViewData["CurrentPage"] = pg ?? 1;
        ViewData["CurrentSearch"] = search;

        int? total = list.Count();
        int? defaultSize = SupplierListConfig.DefaultPageSize;
        size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
        ViewData["CurrentSize"] = size;

        // Tạo trang phân trang
        PaginatedList<SupplierViewModel> result = await PaginatedList<SupplierViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, SupplierListConfig.SearchMessage);

        // Trả về partial view với dữ liệu đã được cập nhật
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
                                   };
                return supplierList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return null;
        }

        //18/03/2024
    }
}