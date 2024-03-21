using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System;
using BDMPro.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
namespace BDMPro.Models
{
    public class Supplier
    {
        [Key]
        [MaxLength(128)]
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ContactId { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class SupplierViewModel
    {
        public string SupplierId { get; set; }
        [Display(Name = "SupplierName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resource))]
        public string SupplierName { get; set; }
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resource))]
        public string Email { get; set; }
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resource))]
        public string Phone { get; set; }
        // public string Address { get; set; }
        // [Display(Name = "Contact", ResourceType = typeof(Resource))]
        // [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resource))]
        // public string ContactId { get; set; }
        // public List<SelectListItem> ContactIdSelectList { get; set; }

        // [Display(Name = "Notes", ResourceType = typeof(Resource))]
        // public string Notes { get; set; }
        // public string EndDateIsoUtc { get; set; }
        // [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        // public DateTime? CreatedOn { get; set; }
        // [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        // public string CreatedOnIsoUtc { get; set; }
        // [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        // public DateTime? ModifiedOn { get; set; }
        // [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        // public string ModifiedOnIsoUtc { get; set; }
        // [Required]
        // [Display(Name = "Status", ResourceType = typeof(Resource))]
        // public string SupplierStatusId { get; set; }
        // [Display(Name = "Status", ResourceType = typeof(Resource))]
        // public string SupplierStatusName { get; set; }
        // [Display(Name = "Status", ResourceType = typeof(Resource))]
        // public List<SelectListItem> SupplierStatusSelectList { get; set; }
        // public List<SelectListItem> ActiveInactiveSelectlist { get; set; }
        // [Display(Name = "Actions", ResourceType = typeof(Resource))]
        // public string Actions { get; set; }
        // public string CreatedBy { get; set; }
        // public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        // [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        // public string IsoUtcCreatedOn { get; set; }
        // [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        // public string ModifiedBy { get; set; }
        // public string IsoUtcModifiedOn { get; set; }
        // public bool? IsActive { get; set; }
        // public bool? IsDeleted { get; set; }
    }

    public class SupplierListing
    {
        public List<SupplierViewModel> Listing { get; set; }
        public List<SelectListItem> ContactIdSelectList { get; set; }
    }
}