using System.ComponentModel.DataAnnotations;
using System;
using BDMPro.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;


namespace YourNamespace.Models
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

        
        }

}
