using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using BDMPro.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace BDMPro.Models
{
    public class SupplierProfile
    {
        [Key]
        [Required]
        [StringLength(128)]
        public string SupplierId { get; set; }

        [Required]
        [StringLength(255)]
        public string SupplierName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(1000)]
        public string Address { get; set; }

        [StringLength(128)]
        public string ContactId { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(128)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        [StringLength(128)]
        public string IsoUtcCreatedOn { get; set; }

        [StringLength(128)]
        public string IsoUtcModifiedOn { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }
    }

    public class SupplierProfileViewModel
    {
        public string SupplierId { get; set; }
        [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Name", ResourceType = typeof(Resource))]
        public string SupplierName { get; set; }
        [Required(ErrorMessageResourceName = "FieldIsRequired", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        public string Email { get; set; }
        [Display(Name = "Contact", ResourceType = typeof(Resource))]
        public string ContactId { get; set; }
        [Display(Name = "Address", ResourceType = typeof(Resource))]
        public string Address { get; set; }
        [Display(Name = "Phone", ResourceType = typeof(Resource))]
        public string Phone { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public String ModifiedBy { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public DateTime? ModifiedOn { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
    }
}