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

        [Required]
        public bool IsDeleted { get; set; }
    }


    public class DeviceTypeViewModel
    {
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

        [Required]
        public bool IsDeleted { get; set; }

        public string Actions { get; set; }

        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }

    }
    public class DeviceTypeListing
    {
        public List<DeviceTypeViewModel> Listing { get; set; }
    }
}