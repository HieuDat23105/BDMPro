using System;
using System.ComponentModel.DataAnnotations;

namespace BDMPro.Models
{
    public class Module
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string MainUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
    }
}