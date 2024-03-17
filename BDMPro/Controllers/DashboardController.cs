using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BDMPro.Models;
using BDMPro.Utils;
using BDMPro.Data;
using BDMPro.Services;
using System.Reflection;

namespace BDMPro.Controllers
{
    [Authorize]
    public class DashboardController : Microsoft.AspNetCore.Mvc.Controller
    {
        private DefaultDBContext db;
        private Util util;
        private ErrorLoggingService _logger;
        public DashboardController(DefaultDBContext db, Util util, ErrorLoggingService logger)
        {
            this.db = db;
            this.util = util;
            _logger = logger;
        }

        // GET: Dashboard
        //Chỉ người dùng có chế độ xem = true cho Dashboard mới có thể truy cập

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Dashboard, "true", "", "", "")]
        public IActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel();
            try
            {
                model.TotalUser = db.AspNetUsers.Select(a => a.Id).Count();
                model.TotalRole = db.AspNetRoles.Select(a => a.Id).Count();
                model.UserByStatus = (from t1 in db.UserProfiles
                                      join t2 in db.GlobalOptionSets on t1.UserStatusId equals t2.Id
                                      where t2.Status == "Active"
                                      group t2.DisplayName by t2.DisplayName into g
                                      select new Chart
                                      {
                                          DataValue = g.Count(),
                                          DataLabel = g.Key
                                      }).ToList();
                var userByRole = (from t1 in db.AspNetUserRoles
                                  join t2 in db.AspNetRoles on t1.RoleId equals t2.Id
                                  group t2.Name by t2.Name into g
                                  select new Chart
                                  {
                                      DataValue = g.Count(),
                                      DataLabel = g.Key
                                  }).ToList();
                model.TopRole = userByRole.OrderBy(a => a.DataValue).Select(a => a.DataLabel).DefaultIfEmpty("N/A").FirstOrDefault();
                if (model.TopRole != "N/A")
                {
                    model.TopRoleId = db.AspNetRoles.Where(a => a.Name == model.TopRole).Select(a => a.Id).FirstOrDefault();
                }
                model.TopStatus = model.UserByStatus.OrderByDescending(a => a.DataValue).Select(a => a.DataLabel).DefaultIfEmpty("N/A").FirstOrDefault();
                if (model.TopStatus != "N/A")
                {
                    model.TopStatusId = db.GlobalOptionSets.Where(a => a.DisplayName == model.TopStatus && a.Type == "UserStatus").Select(a => a.Id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult GetUserByRole()
        {
            List<Chart> chartList = new List<Chart>();
            try
            {
                chartList = (from t1 in db.AspNetRoles
                             join t2 in db.AspNetUserRoles on t1.Id equals t2.RoleId
                             group t1.Name by t1.Name into g
                             select new Chart
                             {
                                 DataValue = g.Count(),
                                 DataLabel = g.Key
                             }).ToList();
                return Ok(chartList.OrderBy(o => o.DataValue));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return Ok(chartList);
        }

        [HttpPost]
        public IActionResult GetUserByStatus()
        {
            List<Chart> userByStatus = new List<Chart>();
            try
            {
                userByStatus = (from t1 in db.UserProfiles
                                join t2 in db.GlobalOptionSets on t1.UserStatusId equals t2.Id
                                where t2.Status == "Active"
                                group t2.DisplayName by t2.DisplayName into g
                                select new Chart
                                {
                                    DataValue = g.Count(),
                                    DataLabel = g.Key
                                }).ToList();
                return Ok(userByStatus.OrderBy(o => o.DataValue));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return Ok(userByStatus);
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