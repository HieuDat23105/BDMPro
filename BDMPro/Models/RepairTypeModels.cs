using BDMPro.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BDMPro.Models
{
     public class RepairType
    {
        [Key]
        [Required]
        [StringLength(128)]
        public string RepairTypeId { get; set; }
        [StringLength(255)]
        public string RepairTypeName { get; set; }
        [Required]
        [StringLength(5)]
        public string RepairTypeSymbol { get; set; }

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
    }


    public class RepairTypeViewModel
    {
        public string RepairTypeId { get; set; }
        [Display(Name = "TypeName", ResourceType = typeof(Resource))]
        public string RepairTypeName { get; set; }
        [Display(Name = "TypeSymbol", ResourceType = typeof(Resource))]
        public string RepairTypeSymbol { get; set; }
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
        [Display(Name = "RepairCount", ResourceType = typeof(Resource))]
        public int RepairCount { get; set; }
    }

    public class RepairTypeListing
    {
        public List<RepairTypeViewModel> Listing { get; set; }
    }
}