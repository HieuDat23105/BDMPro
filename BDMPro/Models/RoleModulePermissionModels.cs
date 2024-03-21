using System;
using System.ComponentModel.DataAnnotations;

namespace BDMPro.Models
{
    public class RoleModulePermission
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string RoleId { get; set; }
        [MaxLength(128)]
        public string ModuleId { get; set; }
        public bool ViewRight { get; set; }
        public bool AddRight { get; set; }
        public bool EditRight { get; set; }
        public bool DeleteRight { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
    }

    public class CurrentUserPermission
    {
        public bool ViewRight { get; set; }
        public bool AddRight { get; set; }
        public bool EditRight { get; set; }
        public bool DeleteRight { get; set; }
    }
}