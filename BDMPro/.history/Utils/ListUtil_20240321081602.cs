using BDMPro.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using BDMPro.Models;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System;
using System.Globalization;

namespace BDMPro.Utils
{
    public static class ListUtil
    {
        public static string DateTimeColumnWidth = "170px";
        public static string IntColumnWidth = "170px";

        public static List<SelectListItem> GetPageSizeDropDownList(int? defaultValue)
        {
            List<SelectListItem> pageSizeDropDownList = new List<SelectListItem>
            {
                new SelectListItem { Text = Resource.Show10Records, Value = "10", Selected = defaultValue == 10 ? true : false },
                new SelectListItem { Text = Resource.Show25Records, Value = "25", Selected = defaultValue == 25 ? true : false },
                new SelectListItem { Text = Resource.Show50Records, Value = "50", Selected = defaultValue == 50 ? true : false },
                new SelectListItem { Text = Resource.ShowAll, Value = "-1", Selected = defaultValue == -1 ? true : false }
            };
            return pageSizeDropDownList;
        }

        public static ColumnHeader[] GenerateDefaultColumnHeaders<T>(string defaultSortOrder, List<string> TableColumns) where T : class
        {
            var headers = new List<ColumnHeader>();
            int count = 1;
            foreach (var property in TableColumns)
            {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Index = count;
                columnHeader.Key = property;
                var parameter = Expression.Parameter(typeof(T), "s");

                var propertyParts = property.Split('.');
                if (propertyParts.Length > 1)
                {
                    // The property is nested (example: StudentViewModel.StudentNameModel.Value), so we need to generate a chain of property expressions
                    Expression memberExpression = parameter;
                    foreach (var part in propertyParts)
                    {
                        memberExpression = Expression.Property(memberExpression, part);
                    }
                    var displayAttribute = (DisplayAttribute)memberExpression.GetMemberExpressions().FirstOrDefault().Member.GetCustomAttributes(typeof(DisplayAttribute), false).SingleOrDefault();
                    if (displayAttribute != null)
                    {
                        var displayName = displayAttribute.GetName();
                        columnHeader.Title = displayName;
                    }
                }
                else
                {
                    var memberExpression = Expression.Property(parameter, property);
                    var displayAttribute = (DisplayAttribute)memberExpression.Member.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
                    var propertyName = displayAttribute != null ? displayAttribute.Name : property;
                    string displayName = Resource.ResourceManager.GetString(propertyName, Resource.Culture);
                    columnHeader.Title = displayName;
                }

                if (defaultSortOrder.Contains(property))
                {
                    string[] segments = defaultSortOrder.Split('-');
                    string order = segments.Last();
                    columnHeader.OrderAction = order == "desc" ? $"{property}-asc" : $"{property}-desc";
                }
                else
                {
                    columnHeader.OrderAction = $"{property}-desc";
                }

                headers.Add(columnHeader);
                count++;
            }
            return headers.ToArray();
        }

        public static IQueryable<T> PerformSort<T>(IQueryable<T> list, string defaultSortOrder, string sort) where T : class
        {
            if (string.IsNullOrEmpty(sort))
            {
                sort = defaultSortOrder;
            }

            string[] segments = sort.Split('-');
            string column = segments[0];
            string direction = segments.Length > 1 ? segments[1] : "desc";

            PropertyInfo propertyInfo;
            Expression propertyExpression;
            var parameter = Expression.Parameter(typeof(T), "s");

            if (column == "Actions") // If sorting by the Actions column, exclude it from sorting
            {
                propertyInfo = typeof(object).GetProperty("GetType"); // Use a dummy property to avoid sorting by Actions
                propertyExpression = Expression.Property(Expression.Constant(new object()), propertyInfo);
            }
            else // Otherwise, sort by the specified column
            {
                propertyInfo = typeof(T).GetProperty(column);
                propertyExpression = Expression.Property(parameter, propertyInfo);
            }

            var lambda = Expression.Lambda(propertyExpression, parameter);

            string methodName = direction == "desc" ? "OrderByDescending" : "OrderBy";

            var result = typeof(Queryable).GetMethods()
                .Single(method => method.Name == methodName &&
                                   method.IsGenericMethodDefinition &&
                                   method.GetGenericArguments().Length == 2 &&
                                   method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), propertyInfo.PropertyType)
                .Invoke(null, new object[] { list, lambda });

            return (IQueryable<T>)result;
        }

        public static List<ColumnHeader> GetColumnHeaders(ColumnHeader[] DefaultColumnHeaders, string sort)
        {
            List<ColumnHeader> headers = new List<ColumnHeader>();
            foreach (var header in DefaultColumnHeaders)
            {
                if (!string.IsNullOrEmpty(header.OrderAction))
                {
                    header.OrderAction = (sort == $"{header.Key}-asc") ? $"{header.Key}-desc" : $"{header.Key}-asc";
                }
                headers.Add(header);
            }
            return headers;
        }

        public static DateTime? ConvertIsoUtcStringToDateTime(string isoUtc)
        {
            if (!string.IsNullOrEmpty(isoUtc))
            {
                string isoString = isoUtc.Substring(0, 16);
                DateTime result = DateTime.ParseExact(isoString, "yyyy-MM-ddTHH:mm", null);
                return result;
            }
            return null;
        }
    }

    public class LoginHistoryListConfig
    {
        public static readonly List<string> TableColumns = new List<string>() {
                nameof(LoginHistoryViewModel.Username),
                nameof(LoginHistoryViewModel.FullName),
                nameof(LoginHistoryViewModel.LoginDateTime)
            };

        //searchMessage = the placeholder for the search bar, from here, you can set different placeholder for different listconfig
        public static string SearchMessage = $"{Resource.Search}...";
        public static IQueryable<LoginHistoryViewModel> PerformSearch(IQueryable<LoginHistoryViewModel> list, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                //perform search (Date-Time fields are not able to be searched because of different time zones for every user. We will need a date time picker in view.cshtml for searching Date Time fields; this feature will be added in a future update.)
                list = list.Where(s => s.Username.Contains(search) || s.FullName.Contains(search) || s.IsoUtcLoginDateTime.Contains(search));
            }
            return list;
        }

        public static IQueryable<LoginHistoryViewModel> PerformSearchDateTime(IQueryable<LoginHistoryViewModel> list, string search, double offset)
        {
            if (!string.IsNullOrEmpty(search))
            {
                List<LoginHistoryViewModel> filteredList = list.ToList();
                List<string> validIds = new List<string>();
                validIds = filteredList.Where(a => ListUtil.ConvertIsoUtcStringToDateTime(a.IsoUtcLoginDateTime).Value.AddMinutes(-offset).ToString("o", CultureInfo.InvariantCulture).Contains(search)).Select(a => a.Id).ToList();
                list = list.Where(s => validIds.Contains(s.Id));
            }
            return list;
        }

        public static string DefaultSortOrder = $"{nameof(LoginHistoryViewModel.LoginDateTime)}-desc";
        public static int? DefaultPageSize = 10;
        public static List<SelectListItem> PageSizeDropDownList = ListUtil.GetPageSizeDropDownList(DefaultPageSize);
        public static readonly ColumnHeader[] DefaultColumnHeaders = ListUtil.GenerateDefaultColumnHeaders<LoginHistoryViewModel>(DefaultSortOrder, TableColumns);

        public static IQueryable<LoginHistoryViewModel> PerformSort(IQueryable<LoginHistoryViewModel> list, string sort)
        {
            var result = ListUtil.PerformSort<LoginHistoryViewModel>(list, DefaultSortOrder, sort);
            return (IQueryable<LoginHistoryViewModel>)result;
        }
    }

    public class UserStatusListConfig
    {
        public static readonly List<string> TableColumns = new List<string>() {
                nameof(GlobalOptionSetViewModel.OptionOrder),
                nameof(GlobalOptionSetViewModel.DisplayName),
                nameof(GlobalOptionSetViewModel.Actions)
            };

        //searchMessage = the placeholder for the search bar, from here, you can set different placeholder for different listconfig
        public static string SearchMessage = $"{Resource.Search}...";
        public static IQueryable<GlobalOptionSetViewModel> PerformSearch(IQueryable<GlobalOptionSetViewModel> list, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                //perform search
                list = list.Where(s => s.OptionOrder.ToString().Contains(search) || s.DisplayName.Contains(search));
            }
            return list;
        }

        public static string DefaultSortOrder = $"{nameof(GlobalOptionSetViewModel.OptionOrder)}-asc";
        public static int? DefaultPageSize = 10;
        public static List<SelectListItem> PageSizeDropDownList = ListUtil.GetPageSizeDropDownList(DefaultPageSize);
        public static readonly ColumnHeader[] DefaultColumnHeaders = ListUtil.GenerateDefaultColumnHeaders<GlobalOptionSetViewModel>(DefaultSortOrder, TableColumns);

        public static IQueryable<GlobalOptionSetViewModel> PerformSort(IQueryable<GlobalOptionSetViewModel> list, string sort)
        {
            var result = ListUtil.PerformSort<GlobalOptionSetViewModel>(list, DefaultSortOrder, sort);
            return result;
        }
    }

    public class UserAttachmentTypeListConfig
    {
        public static readonly List<string> TableColumns = new List<string>() {
                nameof(GlobalOptionSetViewModel.OptionOrder),
                nameof(GlobalOptionSetViewModel.DisplayName),
                nameof(GlobalOptionSetViewModel.Actions)
            };

        //searchMessage = the placeholder for the search bar, from here, you can set different placeholder for different listconfig
        public static string SearchMessage = $"{Resource.Search}...";
        public static IQueryable<GlobalOptionSetViewModel> PerformSearch(IQueryable<GlobalOptionSetViewModel> list, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                //perform search
                list = list.Where(s => s.OptionOrder.ToString().Contains(search) || s.DisplayName.Contains(search));
            }
            return list;
        }

        public static string DefaultSortOrder = $"{nameof(GlobalOptionSetViewModel.OptionOrder)}-asc";
        public static int? DefaultPageSize = 10;
        public static List<SelectListItem> PageSizeDropDownList = ListUtil.GetPageSizeDropDownList(DefaultPageSize);
        public static readonly ColumnHeader[] DefaultColumnHeaders = ListUtil.GenerateDefaultColumnHeaders<GlobalOptionSetViewModel>(DefaultSortOrder, TableColumns);

        public static IQueryable<GlobalOptionSetViewModel> PerformSort(IQueryable<GlobalOptionSetViewModel> list, string sort)
        {
            var result = ListUtil.PerformSort<GlobalOptionSetViewModel>(list, DefaultSortOrder, sort);
            return result;
        }
    }

    public class UserListConfig
    {
        public static readonly List<string> TableColumns = new List<string>() {
                nameof(UserProfileViewModel.Username),
                nameof(UserProfileViewModel.FullName),
                nameof(UserProfileViewModel.EmailAddress),
                nameof(UserProfileViewModel.UserStatusName),
                nameof(UserProfileViewModel.UserRoleNameList),
                nameof(UserProfileViewModel.PhoneNumber),
                nameof(UserProfileViewModel.CountryName),
                nameof(UserProfileViewModel.Address),
                nameof(UserProfileViewModel.CreatedOn),
                nameof(UserProfileViewModel.Actions)
            };

        //searchMessage = the placeholder for the search bar, from here, you can set different placeholder for different listconfig
        public static string SearchMessage = $"{Resource.Search}...";
        public static IQueryable<UserProfileViewModel> PerformSearch(IQueryable<UserProfileViewModel> list, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                //perform search (Date-Time fields are not able to be searched because of different time zones for every user. We will need a date time picker in view.cshtml for searching Date Time fields; this feature will be added in a future update.)
                list = list.Where(s => s.Username.Contains(search) || s.FullName.Contains(search) || s.UserStatusName.Contains(search)
                || s.UserRoleNameList.Contains(search)
                || s.PhoneNumber.Contains(search) || s.CountryName.Contains(search) || s.Address.Contains(search));
            }
            return list;
        }

        public static string DefaultSortOrder = $"{nameof(UserProfileViewModel.CreatedOn)}-desc";
        public static int? DefaultPageSize = 10;
        public static List<SelectListItem> PageSizeDropDownList = ListUtil.GetPageSizeDropDownList(DefaultPageSize);
        public static readonly ColumnHeader[] DefaultColumnHeaders = ListUtil.GenerateDefaultColumnHeaders<UserProfileViewModel>(DefaultSortOrder, TableColumns);

        public static IQueryable<UserProfileViewModel> PerformSort(IQueryable<UserProfileViewModel> list, string sort)
        {
            IQueryable<UserProfileViewModel> emptyList = Enumerable.Empty<UserProfileViewModel>().AsQueryable();
            if (sort.Contains("UserRoleNameList"))
            {
                if (sort.Contains("asc"))
                {
                    var sortedList = list.OrderBy(u => u.UserRoleNameList.FirstOrDefault());
                    return sortedList;
                }
                if (sort.Contains("desc"))
                {
                    var sortedList = list.OrderByDescending(u => u.UserRoleNameList.FirstOrDefault());
                    return sortedList;
                }
            }
            else
            {
                var result = ListUtil.PerformSort<UserProfileViewModel>(list, DefaultSortOrder, sort);
                return result;
            }
            return emptyList;
        }
    }

    public class RoleListConfig
    {
        public static readonly List<string> TableColumns = new List<string>() {
                nameof(SystemRoleViewModel.Name),
                nameof(SystemRoleViewModel.Actions)
            };

        //searchMessage = the placeholder for the search bar, from here, you can set different placeholder for different listconfig
        public static string SearchMessage = $"{Resource.Search}...";
        public static IQueryable<SystemRoleViewModel> PerformSearch(IQueryable<SystemRoleViewModel> list, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(s => s.Name.Contains(search));
            }
            return list;
        }

        public static string DefaultSortOrder = $"{nameof(SystemRoleViewModel.Name)}-asc";
        public static int? DefaultPageSize = 10;
        public static List<SelectListItem> PageSizeDropDownList = ListUtil.GetPageSizeDropDownList(DefaultPageSize);
        public static readonly ColumnHeader[] DefaultColumnHeaders = ListUtil.GenerateDefaultColumnHeaders<SystemRoleViewModel>(DefaultSortOrder, TableColumns);

        public static IQueryable<SystemRoleViewModel> PerformSort(IQueryable<SystemRoleViewModel> list, string sort)
        {
            var result = ListUtil.PerformSort<SystemRoleViewModel>(list, DefaultSortOrder, sort);
            return result;
        }

    }

    public class UserInRoleListConfig
    {
        public static readonly List<string> TableColumns = new List<string>() {
                nameof(UserInRoleViewModel.Username),
                nameof(UserInRoleViewModel.FullName)
            };

        //searchMessage = the placeholder for the search bar, from here, you can set different placeholder for different listconfig
        public static string SearchMessage = $"{Resource.Search}...";
        public static IQueryable<UserInRoleViewModel> PerformSearch(IQueryable<UserInRoleViewModel> list, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(s => s.Username.Contains(search) || s.FullName.Contains(search));
            }
            return list;
        }

        public static string DefaultSortOrder = $"{nameof(UserInRoleViewModel.Username)}-asc";
        public static int? DefaultPageSize = 10;
        public static List<SelectListItem> PageSizeDropDownList = ListUtil.GetPageSizeDropDownList(DefaultPageSize);
        public static readonly ColumnHeader[] DefaultColumnHeaders = ListUtil.GenerateDefaultColumnHeaders<UserInRoleViewModel>(DefaultSortOrder, TableColumns);

        public static IQueryable<UserInRoleViewModel> PerformSort(IQueryable<UserInRoleViewModel> list, string sort)
        {
            var result = ListUtil.PerformSort<UserInRoleViewModel>(list, DefaultSortOrder, sort);
            return result;
        }

    }

    public class UserAttachmentListConfig
    {
        public static readonly List<string> TableColumns = new List<string>() {
                nameof(UserAttachmentViewModel.FileName),
                nameof(UserAttachmentViewModel.AttachmentTypeName),
                nameof(UserAttachmentViewModel.UploadedOn),
                nameof(UserAttachmentViewModel.UploadedBy),
                nameof(UserAttachmentViewModel.Actions)
            };

        //searchMessage = the placeholder for the search bar, from here, you can set different placeholder for different listconfig
        public static string SearchMessage = $"{Resource.Search}...";
        public static IQueryable<UserAttachmentViewModel> PerformSearch(IQueryable<UserAttachmentViewModel> list, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                //perform search (Date-Time fields are not able to be searched because of different time zones for every user. We will need a date time picker in view.cshtml for searching Date Time fields; this feature will be added in a future update.)
                list = list.Where(s => s.FileName.Contains(search) || s.AttachmentTypeName.Contains(search) || s.UploadedBy.Contains(search));
            }
            return list;
        }

        public static string DefaultSortOrder = $"{nameof(UserAttachmentViewModel.UploadedOn)}-desc";
        public static int? DefaultPageSize = 10;
        public static List<SelectListItem> PageSizeDropDownList = ListUtil.GetPageSizeDropDownList(DefaultPageSize);
        public static readonly ColumnHeader[] DefaultColumnHeaders = ListUtil.GenerateDefaultColumnHeaders<UserAttachmentViewModel>(DefaultSortOrder, TableColumns);

        public static IQueryable<UserAttachmentViewModel> PerformSort(IQueryable<UserAttachmentViewModel> list, string sort)
        {
            var result = ListUtil.PerformSort<UserAttachmentViewModel>(list, DefaultSortOrder, sort);
            return result;
        }
    }

    public class SupplierListConfig
    {
        public static readonly List<string> TableColumns = new List<string>() {
                nameof(SupplierViewModel.SupplierName),
                nameof(SupplierViewModel.Email),
                nameof(SupplierViewModel.Phone),
                nameof(SupplierViewModel.Address),
                nameof(SupplierViewModel.ContactId),
                nameof(SupplierViewModel.CreatedOn),
                nameof(SupplierViewModel.Actions)
            };

        //searchMessage = the placeholder for the search bar, from here, you can set different placeholder for different listconfig
        public static string SearchMessage = $"{Resource.Search}...";
        public static IQueryable<SupplierViewModel> PerformSearch(IQueryable<SupplierViewModel> list, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(s => s.SupplierName.Contains(search) ||
                                       s.Email.Contains(search) ||
                                       s.Phone.Contains(search) ||
                                       s.Address.Contains(search) ||
                                       s.CreatedOn.Contains(search) ||
                                       s.ContactId.Contains(search));
            }
            return list;
        }
        public static string DefaultSortOrder = $"{nameof(SupplierViewModel.CreatedOn)}-desc";
        public static int? DefaultPageSize = 10;
        public static List<SelectListItem> PageSizeDropDownList = ListUtil.GetPageSizeDropDownList(DefaultPageSize);
        public static readonly ColumnHeader[] DefaultColumnHeaders = ListUtil.GenerateDefaultColumnHeaders<SupplierViewModel>(DefaultSortOrder, TableColumns);
        public static IQueryable<SupplierViewModel> PerformSort(IQueryable<SupplierViewModel> list, string sort)
        {
            var result = ListUtil.PerformSort<SupplierViewModel>(list, DefaultSortOrder, sort);
            return result;
        }

    }

}
