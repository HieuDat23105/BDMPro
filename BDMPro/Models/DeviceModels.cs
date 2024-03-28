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
    public class Device
    {
        [Key]
        [Required(ErrorMessage = "DeviceId is required")]
        [MaxLength(128)]
        public string DeviceId { get; set; }

        [Required(ErrorMessage = "DeviceTypeId is required")]
        [MaxLength(128)]
        public string DeviceTypeId { get; set; }

        [Required(ErrorMessage = "DeviceCode is required")]
        [MaxLength(9)]
        public string DeviceCode { get; set; }

        [Required(ErrorMessage = "DeviceName is required")]
        [MaxLength(255)]
        public string DeviceName { get; set; }

        [MaxLength(255)]
        public string BrandName { get; set; }

        [MaxLength(255)]
        public string Model { get; set; }

        [MaxLength(50)]
        public string SerialNumber { get; set; }

        [MaxLength(50)]
        public string IpAddress { get; set; }

        [MaxLength(50)]
        public string MacAddress { get; set; }

        [MaxLength(255)]
        public string CpuName { get; set; }

        public double? CpuSpeed { get; set; }

        public int? RamMemory { get; set; }

        [MaxLength(255)]
        public string RamType { get; set; }

        public int? RamSpeed { get; set; }

        public int? SsdMemory { get; set; }

        [MaxLength(255)]
        public string SsdType { get; set; }

        public int? SsdSpeed { get; set; }

        public int? HddMemory { get; set; }

        [MaxLength(255)]
        public string HddType { get; set; }

        public int? HddSpeed { get; set; }

        [MaxLength(255)]
        public string Cd { get; set; }

        [MaxLength(128)]
        public string SupplierId { get; set; }

        [Required(ErrorMessage = "PurchaseDate is required")]
        public DateTime PurchaseDate { get; set; }

        [MaxLength(255)]
        public string WarrantyType { get; set; }

        public DateTime? WarrantyDate { get; set; }

        public DateTime? WarrantyEndDate { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        public DateTime? FirstHandoverDate { get; set; }

        public double? Age { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        [MaxLength(128)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [MaxLength(128)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [MaxLength(128)]
        public string IsoUtcCreatedOn { get; set; }

        [MaxLength(128)]
        public string IsoUtcModifiedOn { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsActiveRepair { get; set; }

        [Required(ErrorMessage = "IsDeleted is required")]
        public bool IsDeleted { get; set; }
    }

    public class DeviceViewModel
    {
        public string DeviceId { get; set; }
        [Display(Name = "DeviceName", ResourceType = typeof(Resource))]
        public string DeviceName { get; set; }
        [Display(Name = "DeviceCode", ResourceType = typeof(Resource))]
        public string DeviceCode { get; set; }
        [Display(Name = "DeviceType", ResourceType = typeof(Resource))]
        public string DeviceTypeId { get; set; }
        [Display(Name = "DeviceType", ResourceType = typeof(Resource))]
        public string DeviceType { get; set; }
        [Display(Name = "DeviceType", ResourceType = typeof(Resource))]
        public List<SelectListItem> DeviceTypeNameList { get; set; }
        [Display(Name = "DeviceType", ResourceType = typeof(Resource))]
        public string DeviceTypeName { get; set; }
        [Display(Name = "BrandName", ResourceType = typeof(Resource))]
        public string BrandName { get; set; }
        [Display(Name = "Model", ResourceType = typeof(Resource))]
        public string Model { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(Resource))]
        public string SerialNumber { get; set; }
        [Display(Name = "IpAddress", ResourceType = typeof(Resource))]
        public string IpAddress { get; set; }
        [Display(Name = "MacAddress", ResourceType = typeof(Resource))]
        public string MacAddress { get; set; }
        [Display(Name = "CpuName", ResourceType = typeof(Resource))]
        public string CpuName { get; set; }
        [Display(Name = "CpuSpeed", ResourceType = typeof(Resource))]
        public double? CpuSpeed { get; set; }
        [Display(Name = "RamMemory", ResourceType = typeof(Resource))]
        public int? RamMemory { get; set; }
        [Display(Name = "RamType", ResourceType = typeof(Resource))]
        public string RamType { get; set; }
        [Display(Name = "RamSpeed", ResourceType = typeof(Resource))]
        public int? RamSpeed { get; set; }
        [Display(Name = "SsdMemory", ResourceType = typeof(Resource))]
        public int? SsdMemory { get; set; }
        [Display(Name = "SsdType", ResourceType = typeof(Resource))]
        public string SsdType { get; set; }
        [Display(Name = "SsdSpeed", ResourceType = typeof(Resource))]
        public int? SsdSpeed { get; set; }
        [Display(Name = "HddMemory", ResourceType = typeof(Resource))]
        public int? HddMemory { get; set; }
        [Display(Name = "HddType", ResourceType = typeof(Resource))]
        public string HddType { get; set; }
        [Display(Name = "HddSpeed", ResourceType = typeof(Resource))]
        public int? HddSpeed { get; set; }
        [Display(Name = "Cd", ResourceType = typeof(Resource))]
        public string Cd { get; set; }
        [Display(Name = "Supplier", ResourceType = typeof(Resource))]
        public string SupplierId { get; set; }
        [Display(Name = "Supplier", ResourceType = typeof(Resource))]
        public string DeviceSupplierName { get; set; }
        [Display(Name = "Supplier", ResourceType = typeof(Resource))]
        public List<SelectListItem> DeviceSupplierSelectList { get; set; }
        [Display(Name = "Supplier", ResourceType = typeof(Resource))]
        public string DeviceSupplier { get; set; }
        [Display(Name = "PurchaseDate", ResourceType = typeof(Resource))]
        public DateTime PurchaseDate { get; set; }
        [Display(Name = "WarrantyType", ResourceType = typeof(Resource))]
        public string WarrantyType { get; set; }
        [Display(Name = "WarrantyDate", ResourceType = typeof(Resource))]
        public DateTime? WarrantyDate { get; set; }
        [Display(Name = "WarrantyEndDate", ResourceType = typeof(Resource))]
        public DateTime? WarrantyEndDate { get; set; }
        [Display(Name = "Location", ResourceType = typeof(Resource))]
        public string Location { get; set; }
        [Display(Name = "FirstHandoverDate", ResourceType = typeof(Resource))]
        public DateTime? FirstHandoverDate { get; set; }
        [Display(Name = "Age", ResourceType = typeof(Resource))]
        public double? Age { get; set; }
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
        [Display(Name = "Actions", ResourceType = typeof(Resource))]
        public string Actions { get; set; }
    }

    public class DeviceListing
    {
        public List<DeviceViewModel> Listing { get; set; }
    }

}
