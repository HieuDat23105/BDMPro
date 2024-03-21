using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using BDMPro.Models;
using System.Globalization;
using System.Data;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using BDMPro.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using BDMPro.Data;
using BDMPro.Services;
using System.Reflection;

namespace BDMPro.Controllers
{
    [Authorize]
    public class GeneralController : Microsoft.AspNetCore.Mvc.Controller
    {
        private DefaultDBContext db;
        private IWebHostEnvironment Environment;
        private ErrorLoggingService _logger;
        public GeneralController(DefaultDBContext _db, IWebHostEnvironment _Environment, ErrorLoggingService logger)
        {
            db = _db;
            Environment = _Environment;
            _logger = logger;
        }
        public List<SelectListItem> GetRoles()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                list = (from t1 in db.AspNetRoles
                        orderby t1.Name
                        select new SelectListItem
                        {
                            Text = t1.Name,
                            Value = t1.Name
                        }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return list;
        }

        public bool FilenameExists(string filename)
        {
            bool filenameExist = false;
            filenameExist = db.UserAttachments.Where(a => a.FileName == filename).Any();
            return filenameExist;
        }

        [AllowAnonymous]
        public IActionResult ChangeLanguage(string lang)
        {
            try
            {
                if (lang != null)
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                }
                else
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    lang = "en";
                }

                Response.Cookies.Append("Language", lang);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }

        [AllowAnonymous]
        public IActionResult SwitchMode()
        {
            try
            {
                string currentColorScheme = Request.Cookies["ColorScheme"];
                if (string.IsNullOrEmpty(currentColorScheme))
                {
                    Response.Cookies.Append("ColorScheme", "dark");
                }
                else
                {
                    if (currentColorScheme == "dark")
                    {
                        Response.Cookies.Append("ColorScheme", "light");
                    }
                    else
                    {
                        Response.Cookies.Append("ColorScheme", "dark");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
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
            }
            base.Dispose(disposing);
        }

    }
}