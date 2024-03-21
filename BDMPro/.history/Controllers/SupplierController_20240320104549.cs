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
            // ViewData["RoleSelectList"] = _util.GetDataForDropDownList("", _db.AspNetRoles, a => a.Name, a => a.Name);
            // ViewData["StatusSelectList"] = _util.GetDataForDropDownList("", _db.GlobalOptionSets, a => a.DisplayName, a => a.DisplayName, a => a.Type == "UserStatus");
            // ViewData["CountrySelectList"] = _util.GetDataForDropDownList("", _db.Countries, a => a.Name, a => a.Name);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewSupplier([FromBody] dynamic requestData)
        {
            try
            {
                var list = ReadSupplierList();
                return PartialView("~/Views/Supplier/_MainList.cshtml", list);
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
                                   CreatedOn = t1.CreatedOn.ToString(),
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