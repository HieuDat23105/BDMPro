using Microsoft.AspNetCore.Mvc.Rendering;
using BDMPro.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace BDMPro.Models
{
    public class CreatedAndModifiedViewModel
    {
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedByName { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string FormattedCreatedOn { get; set; }
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedByName { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public string FormattedModifiedOn { get; set; }
        public int? OrderCreatedOn { get; set; }
        public int? OrderModifiedOn { get; set; }
    }

    public class ImportFromExcelError
    {
        public string Row { get; set; }
        public List<string> Errors { get; set; }
    }

    public class UserInRoleListing
    {
        public List<UserInRoleViewModel> Listing { get; set; }
        public string RoleName { get; set; }
    }

    public class UserInRoleViewModel
    {
        public string Username { get; set; }
        [Display(Name = "FullName", ResourceType = typeof(Resource))]
        public string FullName { get; set; }
        public string UserProfileId { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalUser { get; set; }
        public int TotalRole { get; set; }
        public string TopStatus { get; set; }
        public string TopStatusId { get; set; }
        public string TopRole { get; set; }
        public string TopRoleId { get; set; }
        public List<Chart> UserByStatus { get; set; }
    }

    public class Chart
    {
        public string DataLabel { get; set; }
        public int DataValue { get; set; }
    }

    public class FormControlDemo
    {
        public List<SelectListItem> CategoryList { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Biography { get; set; }
    }

    public class ColumnHeader
    {
        public string Title { get; set; }
        public string OrderAction { get; set; } //ASC hoặc Desc
        public int Index { get; set; }
        public string Key { get; set; }
    }

}