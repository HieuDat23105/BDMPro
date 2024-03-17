using BDMPro.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Security.Principal;
using BDMPro.Resources;
using System.Text;
using BDMPro;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using BDMPro.Utils;
using static BDMPro.Models.ProjectEnum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BDMPro.Models
{
    public static class Extensions
    {
        public static TSource _DefaultIfEmpty<TSource>(this TSource source, TSource defaultValue)
        {
            if (typeof(TSource) == typeof(string) && source == null)
            {
                return defaultValue;
            }
            else if (typeof(TSource) == typeof(int) && source == null)
            {
                return defaultValue;
            }
            else
                return source;
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static string ReplaceWhitespace(string input, string replacement)
        {
            Regex sWhitespace = new Regex(@"\s+");
            return sWhitespace.Replace(input, replacement);
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }

    public class UserProfilePictureActionFilter : ActionFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var acc = (Util)context.HttpContext.RequestServices.
            GetService(typeof(Util));

            ClaimsPrincipal currentUser = context.HttpContext.User;
            var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            string userid = currentUserID;
            var controller = context.Controller as Controller;
            controller.ViewBag.Avatar = acc.GetMyProfilePictureName(userid);
            await next();
        }
    }

    public class PasswordValidation : RequiredAttribute
    {
        public PasswordValidation()
        {
            this.ErrorMessage = Resource.InvalidPassword;
        }

        public override bool IsValid(object value)
        {
            string passwordValue = value as string;
            if (!string.IsNullOrEmpty(passwordValue))
            {
                bool hasNonLetterOrDigit, hasDigit, hasUppercase, hasLowercase = false;
                var validator = new DataValidator();
                hasNonLetterOrDigit = validator.HasNonLetterOrDigit(passwordValue);
                hasDigit = validator.HasDigit(passwordValue);
                hasUppercase = validator.HasUppercase(passwordValue);
                hasLowercase = validator.HasLowercase(passwordValue);

                if (passwordValue.Length < 6 || hasNonLetterOrDigit == false || hasDigit == false || hasUppercase == false || hasLowercase == false)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
    }

    public class Max5MBAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var file = value as IFormFile;
            //Tệp là NULL có nghĩa là nó là một trường tùy chọn, không có tệp để xác thực, trả về true
            if (file == null)
            {
                return true;
            }
            //Tệp hơn 5 MB
            if (file.Length > 5242880)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class Max50MBAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var file = value as IFormFile;
            //Tệp là NULL có nghĩa là nó là một trường tùy chọn, không có tệp để xác thực, trả về true
            if (file == null)
            {
                return true;
            }
            //Tệp hơn 50 MB
            if (file.Length > 52428800)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public static class IdentityExtended
    {
        public static CurrentUserPermission IsAllowed(this IIdentity identity, string moduleCode, Util util)
        {
            var currentUserID = ((System.Security.Claims.ClaimsIdentity)identity).Claims
                .FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            string userid = currentUserID;
            CurrentUserPermission currentUserPermission = new CurrentUserPermission();
            currentUserPermission.ViewRight = false;
            currentUserPermission.AddRight = false;
            currentUserPermission.EditRight = false;
            currentUserPermission.DeleteRight = false;
            currentUserPermission = util.GetCurrentUserPermission(userid, moduleCode);
            return currentUserPermission;
        }
    }

    public class CustomAuthorizeFilter : TypeFilterAttribute
    {
        public CustomAuthorizeFilter(ModuleCode module, string viewRight, string addRight, string editRight, string deleteRight) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { module, viewRight, addRight, editRight, deleteRight };
        }

        private class AuthorizeFilter : AuthorizeAttribute, IAuthorizationFilter
        {
            private string _moduleCode;
            private string _viewRight;
            private string _addRight;
            private string _editRight;
            private string _deleteRight;
            private Util _util;
            public AuthorizeFilter(ModuleCode module, string viewRight, string addRight, string editRight, string deleteRight, Util util)
            {
                _moduleCode = module.ToString();
                _viewRight = viewRight;
                _addRight = addRight;
                _editRight = editRight;
                _deleteRight = deleteRight;
                _util = util;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                bool authorized = false;
                ClaimsPrincipal currentUser = context.HttpContext.User;
                var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                string userid = currentUserID;
                CurrentUserPermission currentUserPermission = new CurrentUserPermission();
                currentUserPermission.ViewRight = false;
                currentUserPermission.AddRight = false;
                currentUserPermission.EditRight = false;
                currentUserPermission.DeleteRight = false;

                currentUserPermission = _util.GetCurrentUserPermission(userid, _moduleCode);

                if (!string.IsNullOrEmpty(_viewRight) && currentUserPermission.ViewRight.ToString().ToLower() == _viewRight)
                {
                    authorized = true;
                }

                if (!string.IsNullOrEmpty(_addRight) && currentUserPermission.AddRight.ToString().ToLower() == _addRight)
                {
                    authorized = true;
                }

                if (!string.IsNullOrEmpty(_editRight) && currentUserPermission.EditRight.ToString().ToLower() == _editRight)
                {
                    authorized = true;
                }

                if (!string.IsNullOrEmpty(_deleteRight) && currentUserPermission.DeleteRight.ToString().ToLower() == _deleteRight)
                {
                    authorized = true;
                }
                if (!authorized)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }

}