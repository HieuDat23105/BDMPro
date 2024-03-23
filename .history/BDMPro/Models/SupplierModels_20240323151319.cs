using System.ComponentModel.DataAnnotations;
using System;
using BDMPro.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;


namespace BDMPro.Models
{
    public class Supplier
    {
        [Key]
        [MaxLength(128)]
        public string SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier Name is required.")]
        [MaxLength(255)]
        public string SupplierName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(1000)]
        public string Address { get; set; }

        [MaxLength(128)]
        public string ContactId { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        [MaxLength(128)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;

        [MaxLength(128)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [MaxLength(128)]
        public string IsoUtcCreatedOn { get; set; }

        [MaxLength(128)]
        public string IsoUtcModifiedOn { get; set; }

        [Required(ErrorMessage = "IsActive flag is required.")]
        public bool IsActive { get; set; }x

        [Required(ErrorMessage = "IsDeleted flag is required.")]
        public bool IsDeleted { get; set; }

        [MaxLength(128)]
        public string SupplierStatusId { get; set; }
    }

    public class SupplierViewModel
    {
        public string SupplierId { get; set; }
        [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "SupplierName", ResourceType = typeof(Resource))]
        public string SupplierName { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(Resource))]
        public string EmailAddress { get; set; }
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resource))]
        public string PhoneNumber { get; set; }
        [Display(Name = "FullAddress", ResourceType = typeof(Resource))]
        public string Address { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string CreatedOnIsoUtc { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public DateTime? ModifiedOn { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public string ModifiedOnIsoUtc { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string SupplierStatus { get; set; }
        [Display(Name = "Contact", ResourceType = typeof(Resource))]
        public string ContactId { get; set; }
        public string CreatedBy { get; set; }
        [Display(Name = "Actions", ResourceType = typeof(Resource))]
        public string Actions { get; set; }
        [Required]
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string SupplierStatusId { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string SupplierStatusName { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public List<SelectListItem> SupplierStatusSelectList { get; set; }
        [Display(Name = "Notes", ResourceType = typeof(Resource))]
        public string Notes { get; set; }
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedBy { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        public bool SystemDefault { get; set; }

    }

    public class SupplierListing
    {
        public List<SupplierViewModel> Listing { get; set; }
    }

}
