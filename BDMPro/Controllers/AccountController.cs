using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BDMPro.Models;
using BDMPro.Resources;
using BDMPro.Utils;
using BDMPro.Data;
using BDMPro.Services;
using System.Reflection;

namespace BDMPro.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;
        private readonly IConfiguration _configuration;
        private Util util;
        private DefaultDBContext db;
        private ErrorLoggingService _logger;

        public AccountController(DefaultDBContext _db, UserManager<AspNetUsers> userManager,
                              SignInManager<AspNetUsers> signInManager, Util _util, ErrorLoggingService logger, IConfiguration configuration)
        {
            db = _db;
            _userManager = userManager;
            _signInManager = signInManager;
            util = _util;
            _logger = logger;
            _configuration = configuration;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string userid = "";

                // Nếu đầu vào tên người dùng chứa ký tự "@", có nghĩa là người dùng sử dụng email để đăng nhập
                if (model.UserName.Contains("@"))
                {
                // Chọn tên người dùng của người dùng từ bảng AspNetusers và gán cho model.username vì thay vì email, SignInManager sử dụng tên người dùng để đăng nhập                    model.UserName = db.AspNetUsers.Where(a => a.Email == model.UserName).Select(a => a.UserName).FirstOrDefault()._DefaultIfEmpty("");
                    userid = db.AspNetUsers.Where(a => a.Email == model.UserName).Select(a => a.Id).FirstOrDefault();
                }
                else
                {
                    userid = db.AspNetUsers.Where(a => a.UserName == model.UserName).Select(a => a.Id).FirstOrDefault();
                }

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    util.SaveLoginHistory(userid);
                    return RedirectToAction("index", "dashboard");
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                ModelState.AddModelError("", Resource.InvalidLoginAttempt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                bool.TryParse(_configuration["ShowDemoAccount"], out bool showDemoAccount);
                if (showDemoAccount && (_userManager.GetUserName(User) == "uadmin" || _userManager.GetUserName(User) == "sadmin"))
                {
                    TempData["NotifyFailed"] = Resource.NotAllowToChangePasswordForDemoAccount;
                    return RedirectToAction("index", "dashboard");
                }
                var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                    TempData["NotifySuccess"] = Resource.PasswordChangedSuccessfully;
                    return RedirectToAction("index", "dashboard");
                }
                AddErrors(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return View(model);
        }

        public IActionResult MyProfile()
        {
            UserProfileViewModel model = new UserProfileViewModel();
            try
            {
                string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                model = util.GetCurrentUserProfile(currentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return View(model);
        }

        public IActionResult EditMyProfile()
        {
            UserProfileViewModel model = new UserProfileViewModel();
            try
            {
                string currentUserId = _userManager.GetUserId(User);
                model = util.GetCurrentUserProfile(currentUserId);
                SetupSelectLists(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return View(model);
        }

        public void SetupSelectLists(UserProfileViewModel model)
        {
            model.GenderSelectList = util.GetGlobalOptionSets("Gender", model.GenderId);
            model.CountrySelectList = util.GetDataForDropDownList(model.CountryName, db.Countries, a => a.Name, a => a.Name);
        }

        [HttpPost]
        public IActionResult EditMyProfile(UserProfileViewModel model, IFormFile ProfilePicture)
        {
            try
            {
                ValidateEditMyProfile(model);

                // 3 trường này chỉ có thể được chỉnh sửa bởi quản trị viên hệ thống trong phần quản lý người dùng.
                ModelState.Remove("UserStatusId");
                ModelState.Remove("UserRoleIdList");
                ModelState.Remove("Password");

                if (!ModelState.IsValid)
                {
                    SetupSelectLists(model);
                    return View(model);
                }

                bool result = SaveMyProfile(model);
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
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("myprofile");
        }

        public void ValidateEditMyProfile(UserProfileViewModel model)
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
            }
        }

        public void AssignUserProfileValues(UserProfile userProfile, UserProfileViewModel model)
        {
            userProfile.FullName = model.FullName;
            userProfile.FirstName = model.FirstName;
            userProfile.LastName = model.LastName;
            userProfile.IsoUtcDateOfBirth = model.IsoUtcDateOfBirth;
            //userProfile.DateOfBirth = util.ConvertToSystemTimeZoneDateTime(model.IsoUtcDateOfBirth);
            userProfile.DateOfBirth = model.IsoUtcDateOfBirth != null ? util.ConvertToSystemTimeZoneDateTime(model.IsoUtcDateOfBirth) : null;
            userProfile.PhoneNumber = model.PhoneNumber;
            userProfile.IDCardNumber = model.IDCardNumber;
            userProfile.GenderId = model.GenderId;
            userProfile.CountryName = model.CountryName;
            userProfile.PostalCode = model.PostalCode;
            userProfile.Address = model.Address;
            userProfile.ModifiedBy = model.ModifiedBy;
            userProfile.ModifiedOn = util.GetSystemTimeZoneDateTimeNow();
            userProfile.IsoUtcModifiedOn = util.GetIsoUtcNow();
        }

        public bool SaveMyProfile(UserProfileViewModel model)
        {
            bool result = true;
            if (model != null)
            {
                try
                {
                    model.ModifiedBy = _userManager.GetUserId(User);
                    //Chỉnh sửa
                    if (model.Id != null)
                    {
                        //Lưu thông tin người dùng
                        UserProfile userProfile = db.UserProfiles.FirstOrDefault(a => a.Id == model.Id);
                        AssignUserProfileValues(userProfile, model);
                        db.Entry(userProfile).State = EntityState.Modified;
                        //Lưu vào AspNetUsers và UserProfile
                        db.SaveChanges();
                        if (model.ProfilePicture != null)
                        {
                            string profilePicture = util.GetGlobalOptionSetId(ProjectEnum.UserAttachment.ProfilePicture.ToString(), "UserAttachment");
                            util.SaveUserAttachment(model.ProfilePicture, userProfile.Id, profilePicture, _userManager.GetUserId(User));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                    return false;
                }
            }
            return result;
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            try
            {
                string id = db.AspNetUsers.Select(a => a.Id).FirstOrDefault();
                model.NoUserYet = string.IsNullOrEmpty(id) ? true : false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                bool usernameExist = util.UsernameExists(model.UserName, null);
                bool emailExist = util.EmailExists(model.Email, null);
                if (usernameExist)
                {
                    ModelState.AddModelError("UserName", Resource.UsernameTaken);
                }
                if (emailExist)
                {
                    ModelState.AddModelError("Email", Resource.EmailAddressTaken);
                }

                if (ModelState.IsValid)
                {
                    bool haveUsersInSystem = db.AspNetUsers.Select(a => a.Id).Any();

                    var user = new AspNetUsers { UserName = model.UserName, Email = model.Email };

                    var result = await _userManager.CreateAsync(user, model.Password); //tại người dùng và lưu vào db
                    if (result.Succeeded)
                    {
                        //tạo thông tin người dùng
                        RegisterUserProfile(user.Id);

                        //Gán tất cả người dùng có vai trò mặc định là  người dùng bình thường
                        var assignNormalUserRole = await _userManager.AddToRoleAsync(user, "Normal User");

                        //Nếu chưa có bất kỳ người dùng nào trong hệ thống, đây là người dùng đăng ký đầu tiên thì gán cho người dùng này quyền User Admin
                        //Giả sử người dùng đầu tiên truy cập hệ thống là quản trị viên hệ thống
                        
                        // if (haveUsersInSystem == false)
                        // {
                        //     var assignSystemAdminRole = await _userManager.AddToRoleAsync(user, "User Admin");
                        // }

                        // cách bật xác nhận tài khoản và đặt lại mật khẩu truy cập https://go.microsoft.com/fwlink/?linkid=320771
                        bool.TryParse(_configuration["ConfirmEmailToLogin"], out bool confirmEmailToLogin);
                        if (confirmEmailToLogin)
                        {
                            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);
                            EmailTemplate emailTemplate = util.EmailTemplateForConfirmEmail(user.UserName, callbackUrl);
                            util.SendEmail(user.Email, emailTemplate.Subject, emailTemplate.Body);
                        }

                        ModelState.Clear();
                        TempData["NotifySuccess"] = Resource.RegisterSuccessLoginNow;
                        return RedirectToAction("login", "account");
                    }

                    string errorMessage = "";
                    foreach (var message in result.Errors)
                    {
                        if (message.Description.Contains("Email"))
                        {
                            ModelState.AddModelError("Email", message.Description);
                        }
                        if (message.Description.Contains("UserName"))
                        {
                            ModelState.AddModelError("UserName", message.Description);
                        }
                        if (message.Description.Contains("Password"))
                        {
                            ModelState.AddModelError("Password", message.Description);
                        }
                        if (message.Description.Contains("ConfirmPassword"))
                        {
                            ModelState.AddModelError("ConfirmPassword", message.Description);
                        }
                        errorMessage += message + "\n";
                    }
                    AddErrors(result);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
                return RedirectToAction("register");
            }
        }

        public void RegisterUserProfile(string userId)
        {
            UserProfile userProfile = new UserProfile();
            userProfile.Id = Guid.NewGuid().ToString();
            userProfile.AspNetUserId = userId;
            userProfile.UserStatusId = util.GetGlobalOptionSetId(ProjectEnum.UserStatus.Registered.ToString(), "UserStatus");
            userProfile.CreatedBy = userId;
            userProfile.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
            userProfile.IsoUtcCreatedOn = util.GetIsoUtcNow();
            db.UserProfiles.Add(userProfile);
            db.SaveChanges();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            try
            {
                var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var result = await _userManager.ConfirmEmailAsync(user, code);
                return View(result.Succeeded ? "ConfirmEmail" : "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return View("Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        // Không thông báo rằng người dùng không tồn tại hoặc không được xác nhận.
                        ModelState.Clear();
                        TempData["NotifySuccess"] = Resource.ResetPasswordEmailSent;
                        return RedirectToAction("login", "account");
                    }

                    // cách bật xác nhận tài khoản và đặt lại mật khẩu truy cập https://go.microsoft.com/fwlink/?linkid=320771
                    string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                    EmailTemplate emailTemplate = util.EmailTemplateForForgotPassword(user.UserName, callbackUrl);
                    util.SendEmail(user.Email, emailTemplate.Subject, emailTemplate.Body);

                    ModelState.Clear();
                    TempData["NotifySuccess"] = Resource.ResetPasswordEmailSent;
                    return RedirectToAction("login", "account");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }

            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public IActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    //không thông báo người dùng không tồn tại
                    ModelState.Clear();
                    TempData["NotifySuccess"] = Resource.YourPasswordResetSuccessfully;
                    return RedirectToAction("login", "account");
                }
                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    TempData["NotifySuccess"] = Resource.YourPasswordResetSuccessfully;
                    return RedirectToAction("login", "account");
                }
                TempData["NotifyFailed"] = Resource.FailedToResetPassword;
                AddErrors(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Yêu cầu chuyển hướng đến nhà cung cấp đăng nhập bên ngoài
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public IActionResult ExternalLoginFailure()
        {
            return View();
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

        #region Helpers
        // Được sử dụng để bảo vệ XSRF khi thêm thông tin đăng nhập bên ngoài
        private const string XsrfKey = "XsrfId";

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : UnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }
        }
        #endregion
    }
}