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
        public string Phone { get; set; }

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
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "IsDeleted flag is required.")]
        public bool IsDeleted { get; set; }
    }

    public class SupplierViewModel
    {
        public string SupplierId { get; set; }
        [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Supplier Name", ResourceType = typeof(Resource))]
        public string SupplierName { get; set; }
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        public string Email { get; set; }
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resource))]
        public string Phone { get; set; }
        [Display(Name = "Address", ResourceType = typeof(Resource))]
        public string Address { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string CreatedOnIsoUtc { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public DateTime? ModifiedOn { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public string ModifiedOnIsoUtc { get; set; }
        [Display(Name = "ActiveInactive", ResourceType = typeof(Resource))]
        public string IsActive { get; set; }
        public List<SelectListItem> ActiveInactiveSelectlist { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string SupplierStatus { get; set; }
        [Display(Name = "Contact", ResourceType = typeof(Resource))]
        public string ContactId { get; set; }
        public string CreatedBy { get; set; }
        [Display(Name = "Actions", ResourceType = typeof(Resource))]
        public string Actions { get; set; }
                public List<SelectListItem> SupplierSelectList { get; set; }
    }

    public class SupplierListing
    {
        public List<SupplierViewModel> Listing { get; set; }
    }

}
