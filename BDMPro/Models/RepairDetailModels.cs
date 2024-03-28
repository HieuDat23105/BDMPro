using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using BDMPro.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BDMPro.Models
{
    public class RepairDetail
    {
        [Key]
        [Required]
        [StringLength(128)]
        public string RepairDetailId { get; set; }

        [Required]
        [StringLength(128)]
        public string DeviceId { get; set; }

        [StringLength(1000)]
        public string ErrorCondition { get; set; }

        [Required]
        [StringLength(128)]
        public string RepairTypeId { get; set; }

        public DateTime? ReceptionDate { get; set; }

        public DateTime? RepairStartDate { get; set; }

        public DateTime? Handover { get; set; }

        [StringLength(1000)]
        public string CancellationInfo { get; set; }

        public DateTime? CancellationDate { get; set; }

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

        public bool? IsActive { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }

    public class RepairDetailViewModel
    {
        public string RepairDetailId { get; set; }
        public string DeviceId { get; set; }
        [Display(Name = "ErrorCondition", ResourceType = typeof(Resource))]
        public string ErrorCondition { get; set; }
        [Display(Name = "RepairType", ResourceType = typeof(Resource))]
        public List<SelectListItem> RepairDetailTypeSelectList { get; set; }
        [Display(Name = "RepairType", ResourceType = typeof(Resource))]
        public string RepairDetailTypeName { get; set; }
        [Required]
        [Display(Name = "RepairType", ResourceType = typeof(Resource))]
        public List<string> RepairDetailTypeIdList { get; set; }
        [Display(Name = "RepairType", ResourceType = typeof(Resource))]
        public List<string> RepairDetailTypeNameList { get; set; }
        [Display(Name = "ReceptionDate", ResourceType = typeof(Resource))]
        public DateTime? ReceptionDate { get; set; }
        [Display(Name = "RepairStartDate", ResourceType = typeof(Resource))]
        public DateTime? RepairStartDate { get; set; }
        [Display(Name = "Handover", ResourceType = typeof(Resource))]
        public DateTime? Handover { get; set; }
        [Display(Name = "CancellationInfo", ResourceType = typeof(Resource))]
        public string CancellationInfo { get; set; }
        [Display(Name = "CancellationDate", ResourceType = typeof(Resource))]
        public DateTime? CancellationDate { get; set; }
        [Display(Name = "Notes", ResourceType = typeof(Resource))]
        public string Notes { get; set; }
         [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string CreatedOnIsoUtc { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public DateTime? ModifiedOn { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public string ModifiedOnIsoUtc { get; set; }
        [Display(Name = "Actions", ResourceType = typeof(Resource))]
        public string Actions { get; set; }
         [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedBy { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        public string CreatedBy { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class RepairDetailListing
    {
        public List<RepairDetailViewModel> Listing { get; set; }
    }
}