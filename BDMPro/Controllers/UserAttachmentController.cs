using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BDMPro.Models;
using BDMPro.Resources;
using BDMPro.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BDMPro.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BDMPro.Services;
using System.Reflection;

namespace BDMPro.Controllers
{
    [Authorize]
    public class UserAttachmentController : Controller
    {
        private DefaultDBContext db;
        private Util util;
        private IWebHostEnvironment Environment;
        private ErrorLoggingService _logger;
        public UserAttachmentController(DefaultDBContext db, IWebHostEnvironment _Environment, Util util, ErrorLoggingService logger)
        {
            this.db = db;
            Environment = _Environment;
            this.util = util;
            _logger = logger;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "true", "", "", "")]
        public IActionResult Index(string Id)
        {
            UserAttachmentListing listing = new UserAttachmentListing();
            if (Id != null)
            {
                try
                {
                    UserAttachmentViewModel model = new UserAttachmentViewModel();
                    model = GetViewModelByUserProfileId(Id);
                    listing.Username = model.Username;
                    listing.FullName = model.FullName;
                    listing.UserProfileId = model.UserProfileId;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                }
            }

            return View(listing);
        }

        //Id = UserAttachment Id
        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "true", "true", "")]
        public IActionResult Edit(string Id)
        {
            UserAttachmentViewModel model = new UserAttachmentViewModel();
            if (Id != null)
            {
                model = GetViewModelByAttachmentId(Id);
            }
            SetupSelectLists(model);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(UserAttachmentViewModel model)
        {
            string typeCode = util.GetGlobalOptionSetCode(model.AttachmentTypeId);
            bool validated = false;
            if (typeCode == "ProfilePicture")
            {
                validated = util.ValidateImageFile(model.FileName);
                if (validated == false)
                {
                    ModelState.AddModelError("AttachmentTypeId", Resource.FailedOnlyJpgJpegPngCanBeSetAsProfilePicture);
                }
            }

            if (!ModelState.IsValid)
            {
                model.CreatedAndModified = new CreatedAndModifiedViewModel();
                model.CreatedAndModified = util.GetCreatedAndModified(model.UploadedBy, model.IsoUtcUploadedOn, null, null);
                SetupSelectLists(model);
                return View(model);
            }
            try
            {
                UserAttachment userAttachment = db.UserAttachments.FirstOrDefault(x => x.Id == model.Id);
                if (userAttachment != null)
                {
                    userAttachment.AttachmentTypeId = model.AttachmentTypeId;
                    userAttachment.ModifiedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    userAttachment.ModifiedOn = util.GetSystemTimeZoneDateTimeNow();
                    userAttachment.IsoUtcModifiedOn = util.GetIsoUtcNow();
                    db.Entry(userAttachment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                ModelState.Clear();
                TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }

            return RedirectToAction("index", new { Id = model.UserProfileId });
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "true", "true", "")]
        public IActionResult Upload(string Id)
        {
            UserAttachmentViewModel model = new UserAttachmentViewModel();
            if (Id != null)
            {
                model = GetViewModelByUserProfileId(Id);
            }
            return View(model);
        }

        public void SetupSelectLists(UserAttachmentViewModel model)
        {
            model.UserAttachmentTypeSelectList = util.GetGlobalOptionSets("UserAttachment", model.Id);
        }

        [HttpPost]
        public IActionResult Upload(UserAttachmentViewModel model, IEnumerable<IFormFile> Files)
        {
            long byteCount = 0;
            //Kiểm tra tính hợp lệ của file
            foreach (IFormFile file in Files)
            {
                byteCount += file.Length;
            }

            //Nếu tổng kích thước tệp lớn hơn 50MB
            if (byteCount > 52428800)
            {
                ModelState.AddModelError("Files", Resource.TotalFileSizeCannotBeLargerThan50MB);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                foreach (IFormFile file in Files)
                {
                    //Kiểm tra tệp có sẵn để lưu. 
                    if (file != null)
                    {
                        util.SaveUserAttachment(file, model.UserProfileId, null, User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }
                }

                ModelState.Clear();
                TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }

            return RedirectToAction("index", new { Id = model.UserProfileId });
        }

        public IActionResult SetAttachmentType(string id, string typeid, string upid)
        {
            try
            {
                string typeCode = util.GetGlobalOptionSetCode(typeid);
                bool validated = false;
                UserAttachment userAttachment = db.UserAttachments.FirstOrDefault(x => x.Id == id);
                if (userAttachment != null)
                {
                    if (typeCode == "ProfilePicture")
                    {
                        if (userAttachment.FileName.Contains("jpg", StringComparison.OrdinalIgnoreCase) || userAttachment.FileName.Contains("jpeg", StringComparison.OrdinalIgnoreCase) || userAttachment.FileName.Contains("png", StringComparison.OrdinalIgnoreCase))
                        {
                            validated = true;
                        }
                    }
                    if (validated == false)
                    {
                        TempData["NotifyFailed"] = Resource.FailedOnlyJpgJpegPngCanBeSetAsProfilePicture;
                        return RedirectToAction("index", new { Id = upid });
                    }
                    userAttachment.AttachmentTypeId = typeid;
                    userAttachment.ModifiedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    userAttachment.ModifiedOn = util.GetSystemTimeZoneDateTimeNow();
                    userAttachment.IsoUtcModifiedOn = util.GetIsoUtcNow();
                    db.Entry(userAttachment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index", new { Id = upid });
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "true", "true", "")]
        public FileResult Download(string Id)
        {
            var fileNames = db.UserAttachments.Where(a => a.Id == Id).Select(a => new { UniqueFileName = a.UniqueFileName, FileName = a.FileName }).FirstOrDefault();
            if (fileNames != null)
            {
                string path = Path.Combine(this.Environment.WebRootPath, "UploadedFiles", fileNames.UniqueFileName);
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileNames.FileName);
            }
            return null;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            string upId = "";
            string fileName = "";
            try
            {
                if (Id != null)
                {
                    UserAttachment model = db.UserAttachments.Where(a => a.Id == Id).FirstOrDefault();
                    if (model != null)
                    {
                        upId = model.UserProfileId;
                        fileName = Path.GetFileName(model.FileName);

                        db.UserAttachments.Remove(model);
                        db.SaveChanges();

                        var path = Path.Combine(this.Environment.WebRootPath, "UploadedFiles", fileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                UserAttachment model = db.UserAttachments.Where(a => a.Id == Id).FirstOrDefault();
                if (model == null)
                {
                    TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index", new { Id = upId });
        }

        public UserAttachmentViewModel GetViewModelByUserProfileId(string Id)
        {
            UserAttachmentViewModel model = new UserAttachmentViewModel();
            try
            {
                model = (from t1 in db.UserProfiles
                         where t1.Id == Id
                         select new UserAttachmentViewModel
                         {
                             Id = Id,
                             FullName = t1.FullName,
                             UserProfileId = t1.Id,
                             AspNetUserId = t1.AspNetUserId
                         }).FirstOrDefault();
                model.Username = db.AspNetUsers.Where(a => a.Id == model.AspNetUserId).Select(a => a.UserName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        public UserAttachmentViewModel GetViewModelByAttachmentId(string Id)
        {
            UserAttachmentViewModel model = new UserAttachmentViewModel();
            try
            {
                model = (from t1 in db.UserAttachments
                         join t2 in db.UserProfiles on t1.UserProfileId equals t2.Id
                         where t1.Id == Id
                         select new UserAttachmentViewModel
                         {
                             Id = t1.Id,
                             FullName = t2.FullName,
                             UserProfileId = t2.Id,
                             AspNetUserId = t2.AspNetUserId,
                             AttachmentTypeId = t1.AttachmentTypeId,
                             FileName = t1.FileName,
                             FileUrl = t1.FileUrl,
                             UploadedOn = t1.CreatedOn,
                             UploadedBy = t1.CreatedBy,
                             IsoUtcUploadedOn = t1.IsoUtcCreatedOn
                         }).FirstOrDefault();
                model.Username = db.AspNetUsers.Where(a => a.Id == model.AspNetUserId).Select(a => a.UserName).FirstOrDefault();
                model.AttachmentTypeName = model.AttachmentTypeId != null ? util.GetGlobalOptionSetDisplayName(model.AttachmentTypeId) : "N/A";
                model.CreatedAndModified = new CreatedAndModifiedViewModel();
                model.CreatedAndModified = util.GetCreatedAndModified(model.UploadedBy, model.IsoUtcUploadedOn, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewUserAttachment(string upId, [FromBody] dynamic requestData)
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
                    sort = UserAttachmentListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(UserAttachmentListConfig.DefaultColumnHeaders, sort);
                var list = ReadUserAttachments(upId);
                string searchMessage = UserAttachmentListConfig.SearchMessage;
                list = UserAttachmentListConfig.PerformSearch(list, search);
                list = UserAttachmentListConfig.PerformSort(list, sort);
                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;
                int? total = list.Count();
                int? defaultSize = UserAttachmentListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;
                PaginatedList<UserAttachmentViewModel> result = await PaginatedList<UserAttachmentViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, searchMessage);
                return PartialView("~/Views/UserAttachment/_MainList.cshtml", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return PartialView("~/Views/Shared/Error.cshtml", null);
        }

        public IQueryable<UserAttachmentViewModel> ReadUserAttachments(string upId)
        {
            var list = from t1 in db.UserAttachments.AsNoTracking()
                       join t2 in db.UserProfiles.AsNoTracking() on t1.CreatedBy equals t2.AspNetUserId
                       join t3 in db.GlobalOptionSets on t1.AttachmentTypeId equals t3.Id into g1
                       from t4 in g1.DefaultIfEmpty()
                       where t1.UserProfileId == upId
                       orderby t1.CreatedOn
                       select new UserAttachmentViewModel
                       {
                           Id = t1.Id,
                           FileName = t1.FileName,
                           FileUrl = t1.FileUrl,
                           AttachmentTypeId = t1.AttachmentTypeId,
                           AttachmentTypeName = t4 == null ? "" : t4.DisplayName,
                           UserProfileId = t1.UserProfileId,
                           UploadedOn = t1.CreatedOn,
                           IsoUtcUploadedOn = t1.IsoUtcCreatedOn,
                           UploadedBy = t2.FullName
                       };
            return list;
        }

    }
}