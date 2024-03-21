using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BDMPro.Resources;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace BDMPro.Models
{
    public class AspNetRoles : IdentityRole<string>
    {
        //[Key]
        //[MaxLength(128)]
        //public string Id { get; set; }
        //[MaxLength(256)]
        //public string Name { get; set; }
        //[MaxLength(256)]
        //public string NormalizedName { get; set; }
        //public string ConcurrencyStamp { get; set; }
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        [MaxLength(128)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool SystemDefault { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
    }

    public class SystemRoleViewModel
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(256)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessageResourceName = "OnlyLettersAndNumbers", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "RoleName", ResourceType = typeof(Resource))]
        public string Name { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedBy { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public DateTime? ModifiedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
        public bool SystemDefault { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        [Display(Name = "Dashboard", ResourceType = typeof(Resource))]
        public Permission DashboardPermission { get; set; } //Dashboard
        [Display(Name = "UserStatus", ResourceType = typeof(Resource))]
        public Permission UserStatusPermission { get; set; } //User Status
        [Display(Name = "UserAttachmentType", ResourceType = typeof(Resource))]
        public Permission UserAttachmentTypePermission { get; set; } //User Attachment Type
        [Display(Name = "RoleManagement", ResourceType = typeof(Resource))]
        public Permission RoleManagementPermission { get; set; } //Role Management
        [Display(Name = "UserManagement", ResourceType = typeof(Resource))]
        public Permission UserManagementPermission { get; set; } //User Management
        [Display(Name = "SupplierManagement", ResourceType = typeof(Resource))]
        public Permission SupplierManagementPermission { get; set; } //Supplier Management
        [Display(Name = "DeviceManagement", ResourceType = typeof(Resource))]
        public Permission DeviceManagementPermission { get; set; } //Device Management
        [Display(Name = "DeviceTypeManagement", ResourceType = typeof(Resource))]
        public Permission DeviceTypeManagementPermission { get; set; } //Device Type Management
        [Display(Name = "RepairManagement", ResourceType = typeof(Resource))]
        public Permission RepairManagementPermission { get; set; } //Repair Management
        [Display(Name = "RepairTypeManagement", ResourceType = typeof(Resource))]
        public Permission RepairTypeManagementPermission { get; set; } //Repair Type Management
        [Display(Name = "Statistical", ResourceType = typeof(Resource))]
        public Permission StatisticalPermission { get; set; } //Statistical
        [Display(Name = "LoginHistory", ResourceType = typeof(Resource))]
        public Permission LoginHistoryPermission { get; set; } //Login History
        [Display(Name = "Actions", ResourceType = typeof(Resource))]
        public string Actions { get; set; } //Actions
    }

    public class SystemRoleListing
    {
        public List<SystemRoleViewModel> Listing { get; set; }
        public string IsoLoginDateTime { get; set; }
        public string FormattedDateTime { get; set; }
    }

    public class Permission
    {
        [Display(Name = "ViewList", ResourceType = typeof(Resource))]
        public ViewPermission ViewPermission { get; set; }
        [Display(Name = "Add", ResourceType = typeof(Resource))]
        public AddPermission AddPermission { get; set; }
        [Display(Name = "Edit", ResourceType = typeof(Resource))]
        public EditPermission EditPermission { get; set; }
        [Display(Name = "Delete", ResourceType = typeof(Resource))]
        public DeletePermission DeletePermission { get; set; }
    }

    public class ViewPermission
    {
        public string Type { get; set; }
        public bool IsSelected { get; set; }
    }
    public class AddPermission
    {
        public string Type { get; set; }
        public bool IsSelected { get; set; }
    }
    public class EditPermission
    {
        public string Type { get; set; }
        public bool IsSelected { get; set; }
    }
    public class DeletePermission
    {
        public string Type { get; set; }
        public bool IsSelected { get; set; }
    }
}