using BDMPro.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BDMPro.Models
{
    public class DeviceType
    {
        [Key]
        [Required]
        [StringLength(128)]
        public string DeviceTypeId { get; set; }

        [Required]
        [StringLength(255)]
        public string TypeName { get; set; }

        [Required]
        [StringLength(5)]
        public string TypeSymbol { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [StringLength(128)]
        public string OrderCode { get; set; }

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
    }


    public class DeviceTypeViewModel
    {
        public string DeviceTypeId { get; set; }
        [Display(Name = "TypeName", ResourceType = typeof(Resource))]
        public string TypeName { get; set; }
        [Display(Name = "TypeSymbol", ResourceType = typeof(Resource))]
        public string TypeSymbol { get; set; }
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
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedBy { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Actions { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        [Display(Name = "DeviceCount", ResourceType = typeof(Resource))]
        public int DeviceCount { get; set; }
        [Display(Name = "OrderCode", ResourceType = typeof(Resource))]
        public string OrderCode { get; set; }
    }
    public class DeviceTypeListing
    {
        public List<DeviceTypeViewModel> Listing { get; set; }
    }
}