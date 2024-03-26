using System.ComponentModel.DataAnnotations;
using System;

namespace BDMPro.Models
{
    public class Contact
    { [Key]
        [MaxLength(128)]
        public string ContactId { get; set; }

        [Required(ErrorMessage = "Contact Name is required.")]
        [MaxLength(255)]
        public string ContactName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [MaxLength(255)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [MaxLength(15)]
        public string Phone { get; set; }

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

        [Required(ErrorMessage = "IsActive flag is required.")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "IsDeleted flag is required.")]
        public bool IsDeleted { get; set; }
        public str
    }
}
