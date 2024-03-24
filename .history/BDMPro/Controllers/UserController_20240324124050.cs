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

namespace BDMPro.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;
        private readonly IConfiguration _configuration;
        private DefaultDBContext db;
        private Util util;
        private ErrorLoggingService _logger;
        private IWebHostEnvironment Environment;

        public UserController(DefaultDBContext _db, UserManager<AspNetUsers> userManager,
                              SignInManager<AspNetUsers> signInManager, Util _util, ErrorLoggingService logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            db = _db;
            _userManager = userManager;
            _signInManager = signInManager;
            util = _util;
            _logger = logger;
            _configuration = configuration;
            Environment = environment;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "true", "", "", "")]
        public IActionResult Index()
        {
            ViewData["RoleSelectList"] = util.GetDataForDropDownList("", db.AspNetRoles, a => a.Name, a => a.Name);
            ViewData["StatusSelectList"] = util.GetDataForDropDownList("", db.GlobalOptionSets, a => a.DisplayName, a => a.DisplayName, a => a.Type == "UserStatus");
            ViewData["CountrySelectList"] = util.GetDataForDropDownList("", db.Countries, a => a.Name, a => a.Name);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPartialViewListing([FromBody] dynamic requestData)
        {
            string sort = requestData.sort?.Value;
            int? size = (int.TryParse(requestData.size.Value, out int parsedSize)) ? parsedSize : null;
            string search = requestData.search?.Value;
            int? pg = (int.TryParse(requestData.pg.Value, out int parsedPg)) ? parsedPg : 1;
            string role = requestData.role?.Value;
            string status = requestData.status?.Value;
            string country = requestData.country?.Value;

            try
            {
                List<ColumnHeader> headers = new List<ColumnHeader>();
                if (string.IsNullOrEmpty(sort))
                {
                    sort = UserListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(UserListConfig.DefaultColumnHeaders, sort);
                var list = ReadUserProfileList();
                string searchMessage = UserListConfig.SearchMessage;
                list = UserListConfig.PerformSearch(list, search);
                list = UserListConfig.PerformSearch(list, role);
                list = UserListConfig.PerformSearch(list, status);
                list = UserListConfig.PerformSearch(list, country);
                list = UserListConfig.PerformSort(list, sort);
                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;
                int? total = list.Count();
                int? defaultSize = UserListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;
                PaginatedList<UserProfileViewModel> result = await PaginatedList<UserProfileViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, searchMessage);
                return PartialView("~/Views/User/_MainList.cshtml", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return PartialView("~/Views/Shared/Error.cshtml", null);
        }

        public IQueryable<UserProfileViewModel> ReadUserProfileList()
        {
            var userList = from t1 in db.UserProfiles.AsNoTracking()
                           let t2 = db.AspNetUsers.Where(u => u.Id == t1.AspNetUserId).SingleOrDefault()
                           let t3 = db.GlobalOptionSets.Where(g => g.Id == t1.UserStatusId).SingleOrDefault()
                           select new UserProfileViewModel
                           {
                               Id = t1.Id,
                               FullName = t1.FullName,
                               Username = t2.UserName,
                               AspNetUserId = t1.AspNetUserId,
                               UserStatusName = t3 == null ? "" : t3.DisplayName,
                               EmailAddress = t2.Email,
                               PhoneNumber = t1.PhoneNumber,
                               CountryName = t1.CountryName,
                               Address = t1.Address,
                               CreatedOn = t1.CreatedOn,
                               IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                               UserRoleNameList = (from t4 in db.AspNetUserRoles
                                                   join t5 in db.AspNetRoles on t4.RoleId equals t5.Id
                                                   where t4.UserId == t1.AspNetUserId
                                                   orderby t5.Name
                                                   select t5.Name).Distinct().ToList()
                           };
            return userList;
        }

        public UserProfileViewModel GetUserProfileViewModel(string Id, string type)
        {
            UserProfileViewModel model = new UserProfileViewModel();
            try
            {
                string profilePicTypeId = util.GetGlobalOptionSetId(ProjectEnum.UserAttachment.ProfilePicture.ToString(), "UserAttachment");
                model = (from t1 in db.UserProfiles
                         join t2 in db.AspNetUsers on t1.AspNetUserId equals t2.Id
                         where t1.Id == Id
                         select new UserProfileViewModel
                         {
                             Id = t1.Id,
                             AspNetUserId = t1.AspNetUserId,
                             FullName = t1.FullName,
                             IDCardNumber = t1.IDCardNumber,
                             FirstName = t1.FirstName,
                             LastName = t1.LastName,
                             DateOfBirth = t1.DateOfBirth,
                             PhoneNumber = t1.PhoneNumber,
                             Address = t1.Address,
                             PostalCode = t1.PostalCode,
                             Username = t2.UserName,
                             EmailAddress = t2.Email,
                             UserStatusId = t1.UserStatusId,
                             GenderId = t1.GenderId,
                             CountryName = t1.CountryName,
                             CreatedBy = t1.CreatedBy,
                             ModifiedBy = t1.ModifiedBy,
                             CreatedOn = t1.CreatedOn,
                             ModifiedOn = t1.ModifiedOn,
                             IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                             IsoUtcModifiedOn = t1.IsoUtcModifiedOn,
                             IsoUtcDateOfBirth = t1.IsoUtcDateOfBirth
                         }).FirstOrDefault();
                model.UserStatusName = db.GlobalOptionSets.Where(a => a.Id == model.UserStatusId).Select(a => a.DisplayName).FirstOrDefault();
                model.UserRoleIdList = (from t1 in db.AspNetUserRoles
                                        join t2 in db.AspNetRoles on t1.RoleId equals t2.Id
                                        where t1.UserId == model.AspNetUserId
                                        select t2.Name).ToList();
                model.UserRoleName = String.Join(", ", model.UserRoleIdList);
                model.GenderName = db.GlobalOptionSets.Where(a => a.Id == model.GenderId).Select(a => a.DisplayName).FirstOrDefault();
                model.ProfilePictureFileName = db.UserAttachments.Where(a => a.UserProfileId == model.Id && a.AttachmentTypeId == profilePicTypeId).OrderByDescending(a => a.CreatedOn).Select(a => a.UniqueFileName).FirstOrDefault();
                if (type == "View")
                {
                    model.CreatedAndModified = util.GetCreatedAndModified(model.CreatedBy, model.IsoUtcCreatedOn, model.ModifiedBy, model.IsoUtcModifiedOn);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        public void SetupSelectLists(UserProfileViewModel model)
        {
            model.GenderSelectList = util.GetDataForDropDownList(model.GenderId, db.GlobalOptionSets, a => a.DisplayName, a => a.Id, a => a.Type == "Gender", a => a.OptionOrder, "asc");
            model.UserStatusSelectList = util.GetDataForDropDownList(model.UserStatusId, db.GlobalOptionSets, a => a.DisplayName, a => a.Id, a => a.Type == "UserStatus", a => a.OptionOrder, "asc");
            model.UserRoleSelectList = util.GetRolesForMultiSelect(model.UserRoleIdList);
            model.CountrySelectList = util.GetDataForDropDownList(model.CountryName, db.Countries, a => a.Name, a => a.Name);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "true", "true", "")]
        public IActionResult Import()
        {
            ImportFromExcel importFromExcel = new ImportFromExcel();
            return View(importFromExcel);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "true", "true", "true", "")]
        public IActionResult DownloadImportTemplate()
        {
            var path = Path.Combine(this.Environment.WebRootPath, "Assets", "UserExcelTemplate.xlsx");
            var tempFilePath = Path.Combine(this.Environment.WebRootPath, "Assets", "ImportUsersFromExcel.xlsx");
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
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, $"ImportUsersFromExcel{dtnow}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> Import(ImportFromExcel model, IFormFile File)
        {
            try
            {
                List<string> errors = new List<string>();
                List<ImportFromExcelError> errorsList = new List<ImportFromExcelError>();
                var users = new List<AspNetUsers>();

                UserProfileViewModel upModel = new UserProfileViewModel();

                int successCount = 0;
                int dtRowsCount = 0;
                List<string> columns = new List<string>();
                using (var memoryStream = new MemoryStream())
                {
                    File.CopyTo(memoryStream);
                    using (var reader = ExcelReaderFactory.CreateReader(memoryStream))
                    {
                        var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        var dt = ds.Tables[0];

                        foreach (var col in dt.Columns.Cast<DataColumn>())
                        {
                            col.ColumnName = col.ColumnName.Replace("*", "");
                            columns.Add(col.ColumnName);
                        }
                        dtRowsCount = dt.Rows.Count;

                        errors = util.ValidateColumns(columns, new List<string>
                        {
                            "Username","Email Address","Password",
                            "Confirm Password","Full Name","First Name",
                            "Last Name","Phone Number","Country",
                        });

                        if (errors.Count == 0)
                        {
                            for (int i = 0; i < dtRowsCount; i++)
                            {
                                try
                                {
                                    string userName = dt.Rows[i].Field<string>("Username");
                                    string email = dt.Rows[i].Field<string>("Email Address");
                                    string pass = dt.Rows[i].Field<string>("Password");
                                    string confirm_pass = dt.Rows[i].Field<string>("Confirm Password");
                                    string fullName = dt.Rows[i].Field<string>("Full Name");
                                    //string phone = dt.Rows[i].Field<string>("Phone Number");
                                    object phoneNumberObject = dt.Rows[i]["Phone Number"];
                                    string phone = Convert.ToString(phoneNumberObject);
                                    string country = dt.Rows[i].Field<string>("Country");
                                    string firstName = dt.Rows[i].Field<string>("First Name");
                                    string lastName = dt.Rows[i].Field<string>("Last Name");

                                    upModel.FullName = fullName;
                                    upModel.PhoneNumber = phone;
                                    upModel.Password = pass;
                                    upModel.ConfirmPassword = confirm_pass;
                                    upModel.EmailAddress = email;
                                    upModel.Username = userName;
                                    upModel.CountryName = country;
                                    upModel.FirstName = firstName;
                                    upModel.LastName = lastName;

                                    errors = util.ValidateImportUserFromExcel(upModel);

                                    if (errors.Count() > 0)
                                    {
                                        ImportFromExcelError importFromExcelError = new ImportFromExcelError();
                                        importFromExcelError.Row = $"At Row {i + 2}";
                                        importFromExcelError.Errors = errors;
                                        errorsList.Add(importFromExcelError);
                                        continue;
                                    }

                                    var user = new AspNetUsers { UserName = upModel.Username, Email = upModel.EmailAddress };
                                    var creationResult = await _userManager.CreateAsync(user, upModel.Password);
                                    if (creationResult.Succeeded)
                                    {
                                        UserProfile userProfile = new UserProfile();
                                        userProfile.Id = Guid.NewGuid().ToString();

                                        userProfile.AspNetUserId = user.Id;
                                        userProfile.FullName = upModel.FullName;
                                        userProfile.FirstName = upModel.FirstName;
                                        userProfile.LastName = upModel.LastName;
                                        userProfile.PhoneNumber = upModel.PhoneNumber;
                                        userProfile.CountryName = upModel.CountryName;
                                        userProfile.UserStatusId = util.GetGlobalOptionSetId(ProjectEnum.UserStatus.Registered.ToString(), "UserStatus");
                                        userProfile.CreatedBy = _userManager.GetUserId(User);
                                        userProfile.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                                        userProfile.IsoUtcCreatedOn = util.GetIsoUtcNow();
                                        db.UserProfiles.Add(userProfile);
                                        db.SaveChanges();

                                        var assignNormalUserRole = await _userManager.AddToRoleAsync(user, "Normal User");

                                        successCount++;
                                    }
                                    ModelState.Clear();
                                }
                                catch (Exception ex)
                                {
                                    errors.Add($"{ex.Message} - Row: {i + 2}");
                                    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                                }
                            }
                        }
                        else
                        {
                            ImportFromExcelError importFromExcelError = new ImportFromExcelError();
                            importFromExcelError.Row = Resource.InvalidUserTemplate;
                            importFromExcelError.Errors = errors;
                            errorsList.Add(importFromExcelError);
                        }
                    }
                }
                if (errorsList.Count > 0)
                {
                    model.ErrorList = errorsList;
                    model.UploadResult = $"{successCount} {Resource.outof} {dtRowsCount} {Resource.recordsuploaded}";
                    return View("import", model);
                }
                TempData["NotifySuccess"] = Resource.RecordsImportedSuccessfully;
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index");
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "true", "true", "")]
        public IActionResult Edit(string Id)
        {
            UserProfileViewModel model = new UserProfileViewModel();
            if (Id != null)
            {
                model = GetUserProfileViewModel(Id, "Edit");
            }
            SetupSelectLists(model);
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "true", "", "", "")]
        public IActionResult ViewRecord(string Id)
        {
            UserProfileViewModel model = new UserProfileViewModel();
            if (Id != null)
            {
                model = GetUserProfileViewModel(Id, "View");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserProfileViewModel model)
        {
            try
            {
                ValidateModel(model);

                if (!ModelState.IsValid)
                {
                    SetupSelectLists(model);
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
            return RedirectToAction("index", "user");
        }

        public void ValidateModel(UserProfileViewModel model)
        {
            if (model != null)
            {
                bool usernameExist = util.UsernameExists(model.Username, model.AspNetUserId);
                bool emailExist = util.EmailExists(model.EmailAddress, model.AspNetUserId);
                if (usernameExist)
                {
                    ModelState.AddModelError("UserName", Resource.UsernameTaken);
                }
                if (emailExist)
                {
                    ModelState.AddModelError("EmailAddress", Resource.EmailAddressTaken);
                }
                if (model.Id == null)
                {
                    if (model.Password == null)
                    {
                        ModelState.AddModelError("Password", Resource.PasswordRequired);
                    }
                    if (model.ConfirmPassword == null)
                    {
                        ModelState.AddModelError("ConfirmPassword", Resource.ConfirmPasswordRequired);
                    }
                }
                else
                {
                    ModelState.Remove("Password");
                    ModelState.Remove("ConfirmPassword");
                }
                if (model.UserStatusId == null || model.UserStatusId == "null")
                {
                    ModelState.AddModelError("UserStatusId", Resource.StatusRequired);
                }
                if (model.UserRoleIdList == null || model.UserRoleIdList.Count() == 0)
                {
                    ModelState.AddModelError("UserRoleIdList", Resource.RoleRequired);
                }
                if (string.IsNullOrEmpty(model.CountryName))
                {
                    ModelState.AddModelError("CountryName", Resource.CountryRequired);
                }
                if (model.ProfilePicture != null)
                {
                    List<string> acceptedFormat = new List<string>() { "jpg", "jpeg", "png", "JPG", "JPEG", "PNG" };
                    if (acceptedFormat.Contains(Path.GetExtension(model.ProfilePicture.FileName).TrimStart('.')) == false)
                    {
                        ModelState.AddModelError("ProfilePicture", Resource.FailedOnlyJpgJpegPngCanBeSetAsProfilePicture);
                    }
                }
            }
        }

        public void AssignUserProfileValues(UserProfile userProfile, UserProfileViewModel model)
        {
            userProfile.FullName = model.FullName;
            userProfile.FirstName = model.FirstName;
            userProfile.LastName = model.LastName;

            userProfile.IsoUtcDateOfBirth = model.IsoUtcDateOfBirth;
            userProfile.DateOfBirth = model.IsoUtcDateOfBirth != null ? util.ConvertToSystemTimeZoneDateTime(model.IsoUtcDateOfBirth) : null;

            userProfile.PhoneNumber = model.PhoneNumber;
            userProfile.IDCardNumber = model.IDCardNumber;
            userProfile.GenderId = model.GenderId;
            userProfile.CountryName = model.CountryName;
            userProfile.PostalCode = model.PostalCode;
            userProfile.Address = model.Address;
            userProfile.UserStatusId = string.IsNullOrEmpty(model.UserStatusId) ? util.GetGlobalOptionSetId(UserStatus.Registered.ToString(), "UserStatus") : model.UserStatusId;
            if (model.Id == null)
            {
                userProfile.CreatedBy = model.CreatedBy;
                userProfile.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                userProfile.IsoUtcCreatedOn = util.GetIsoUtcNow();
            }
            else
            {
                userProfile.ModifiedBy = model.ModifiedBy;
                userProfile.ModifiedOn = util.GetSystemTimeZoneDateTimeNow();
                userProfile.IsoUtcModifiedOn = util.GetIsoUtcNow();
            }
        }

        public async Task<bool> SaveRecord(UserProfileViewModel model)
        {
            bool result = true;
            if (model != null)
            {
                string userId = "";
                string userProfileId = "";
                string profilePictureId = "";
                string type = "";
                try
                {
                    model.CreatedBy = _userManager.GetUserId(User);
                    model.ModifiedBy = _userManager.GetUserId(User);
                    if (model.Id != null)
                    {
                        type = "update";
                        AspNetUsers aspNetUsers = db.AspNetUsers.FirstOrDefault(a => a.Id == model.AspNetUserId);
                        aspNetUsers.UserName = model.Username;
                        aspNetUsers.Email = model.EmailAddress;
                        db.Entry(aspNetUsers).State = EntityState.Modified;

                        UserProfile userProfile = db.UserProfiles.FirstOrDefault(a => a.Id == model.Id);
                        AssignUserProfileValues(userProfile, model);
                        db.Entry(userProfile).State = EntityState.Modified;

                        db.SaveChanges();

                        userProfileId = userProfile.Id;

                        if (model.UserRoleIdList != null)
                        {
                            var user = await _userManager.FindByIdAsync(model.AspNetUserId);
                            var existingRoles = await _userManager.GetRolesAsync(user);
                            string[] removeRoles = existingRoles.ToArray();
                            var removeRole = await _userManager.RemoveFromRolesAsync(user, removeRoles);
                            string[] roles = model.UserRoleIdList.ToArray();
                            var assignRole = await _userManager.AddToRolesAsync(user, roles);
                        }

                    }
                    else
                    {
                        type = "create";
                        var user = new AspNetUsers { UserName = model.Username, Email = model.EmailAddress };
                        //var creationResult = account.CreateNewUserIdentity(user, model.Password);
                        if (creationResult.Succeeded)
                        {
                            userId = user.Id;
                            if (model.UserRoleIdList != null)
                            {
                                string[] roles = model.UserRoleIdList.ToArray();
                                var assignRoleResult = await _userManager.AddToRolesAsync(user, roles);
                            }

                            //save user profile
                            UserProfile userProfile = new UserProfile();
                            AssignUserProfileValues(userProfile, model);
                            userProfile.Id = Guid.NewGuid().ToString();
                            userProfile.AspNetUserId = user.Id;
                            db.UserProfiles.Add(userProfile);
                            db.SaveChanges();
                            userProfileId = userProfile.Id;

                            // Send an email with this link
                            bool.TryParse(_configuration["ConfirmEmailToLogin"], out bool confirmEmailToLogin);
                            if (confirmEmailToLogin)
                            {
                                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);
                                EmailTemplate emailTemplate = util.EmailTemplateForConfirmEmail(user.UserName, callbackUrl);
                                util.SendEmail(user.Email, emailTemplate.Subject, emailTemplate.Body);
                            }
                        }
                    }

                    if (model.ProfilePicture != null)
                    {
                        string profilePicture = util.GetGlobalOptionSetId(ProjectEnum.UserAttachment.ProfilePicture.ToString(), "UserAttachment");
                        util.SaveUserAttachment(model.ProfilePicture, userProfileId, profilePicture, _userManager.GetUserId(User));
                    }

                }
                catch (Exception ex)
                {
                    //Exception when creating new record, means record creation incomplete due to error, undo the record creation
                    if (type == "create")
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            AspNetUsers aspNetUsers = db.AspNetUsers.FirstOrDefault(a => a.Id == userId);
                            if (aspNetUsers != null)
                            {
                                db.AspNetUsers.Remove(aspNetUsers);
                                db.SaveChanges();
                            }
                        }
                        if (!string.IsNullOrEmpty(userProfileId))
                        {
                            UserProfile userProfile = db.UserProfiles.FirstOrDefault(a => a.Id == userProfileId);
                            if (userProfile != null)
                            {
                                db.UserProfiles.Remove(userProfile);
                                db.SaveChanges();
                            }
                        }
                        if (!string.IsNullOrEmpty(profilePictureId))
                        {
                            UserAttachment userAttachment = db.UserAttachments.FirstOrDefault(a => a.Id == profilePictureId);
                            if (userAttachment != null)
                            {
                                db.UserAttachments.Remove(userAttachment);
                                db.SaveChanges();
                            }
                        }
                    }
                    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                    return false;
                }
            }
            return result;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "true", "true", "true", "")]
        public IActionResult AdminChangePassword(string Id)
        {
            AdminChangePasswordViewModel model = new AdminChangePasswordViewModel();
            if (Id != null)
            {
                model = GetAdminChangePasswordViewModel(Id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AdminChangePassword(AdminChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                //Not allowed to change demo account's password
                string username = db.AspNetUsers.Where(a => a.Id == model.AspNetUserId).Select(a => a.UserName).FirstOrDefault();
                bool.TryParse(_configuration["ShowDemoAccount"], out bool showDemoAccount);
                if (showDemoAccount && username == "nsadmin")
                {
                    TempData["NotifyFailed"] = Resource.NotAllowToChangePasswordForDemoAccount;
                    return RedirectToAction("index", "dashboard");
                }

                var userExists = await _userManager.FindByIdAsync(model.AspNetUserId);
                if (userExists != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(userExists);
                    var result = await _userManager.ResetPasswordAsync(userExists, token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        ModelState.Clear();
                        TempData["NotifySuccess"] = Resource.PasswordChangedSuccessfully;

                        //send email to notify the user
                        string userEmail = db.AspNetUsers.Where(a => a.Id == model.AspNetUserId).Select(a => a.Email).FirstOrDefault();
                        string resetById = _userManager.GetUserId(User);
                        string resetByName = db.UserProfiles.Where(a => a.AspNetUserId == resetById).Select(a => a.FullName).FirstOrDefault();
                        EmailTemplate emailTemplate = util.EmailTemplateForPasswordResetByAdmin(model.Username, resetByName, model.NewPassword);
                        util.SendEmail(userEmail, emailTemplate.Subject, emailTemplate.Body);
                    }
                    else
                    {
                        TempData["NotifyFailed"] = Resource.FailedToResetPassword;
                    }
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.UserNotExist;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index", "user");
        }

        public AdminChangePasswordViewModel GetAdminChangePasswordViewModel(string Id)
        {
            AdminChangePasswordViewModel model = new AdminChangePasswordViewModel();
            try
            {
                model = (from t1 in db.UserProfiles
                         join t2 in db.AspNetUsers on t1.AspNetUserId equals t2.Id
                         where t1.Id == Id
                         select new AdminChangePasswordViewModel
                         {
                             Id = Id,
                             FullName = t1.FullName,
                             Username = t2.UserName,
                             EmailAddress = t2.Email,
                             AspNetUserId = t1.AspNetUserId
                         }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return model;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.UserManagement, "", "", "", "true")]
        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    AspNetUsers users = db.AspNetUsers.Where(a => a.Id == Id).FirstOrDefault();
                    if (users != null)
                    {
                        bool.TryParse(_configuration["ShowDemoAccount"], out bool showDemoAccount);
                        if (showDemoAccount && users.UserName == "nsadmin")
                        {
                            TempData["NotifyFailed"] = Resource.DemoAccountCannotBeDeleted;
                            return RedirectToAction("index");
                        }
                        db.AspNetUsers.Remove(users);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                AspNetUsers users = db.AspNetUsers.Where(a => a.Id == Id).FirstOrDefault();
                if (users == null)
                {
                    TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index");
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