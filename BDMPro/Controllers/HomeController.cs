using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BDMPro.Models;
using BDMPro.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BDMPro.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using BDMPro.Services;
using System.Reflection;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace BDMPro.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private IWebHostEnvironment Environment;
        private DefaultDBContext db;
        private IConfiguration Configuration;
        private ErrorLoggingService _logger;

        public HomeController(IWebHostEnvironment environment, DefaultDBContext _db, IConfiguration _Configuration, ErrorLoggingService logger)
        {
            Environment = environment;
            db = _db;
            Configuration = _Configuration;
            _logger = logger;
        }

        public IActionResult UnauthorizedAccess()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("UnauthorizedTwo");
            }
            return RedirectToAction("UnauthorizedOne");
        }
        public IActionResult UnauthorizedOne()
        {
            ViewBag.Message = Resource.YouDontHavePermissionToAccess;
            return View();
        }
        public IActionResult UnauthorizedTwo()
        {
            ViewBag.Message = Resource.YouDontHavePermissionToAccess;
            return View();
        }
    }
}