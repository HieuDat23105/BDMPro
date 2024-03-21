using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDMPro.Models
{
    public class AspNetUsers : IdentityUser<string>
    {
        //[Key]
        //[MaxLength(450)]
        //public string Id { get; set; }
        //[MaxLength(256)]
        //public string UserName { get; set; }
        //[MaxLength(256)]
        //public string NormalizedUserName { get; set; }
        //[MaxLength(256)]
        //public string Email { get; set; }
        //[MaxLength(256)]
        //public string NormalizedEmail { get; set; }
        //public bool EmailConfirmed { get; set; }
        //public string PasswordHash { get; set; }
        //public string SecurityStamp { get; set; }
        //public string PhoneNumber { get; set; }
        //public bool PhoneNumberConfirmed { get; set; }
        //public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        //public DateTimeOffset? LockoutEnd { get; set; }
        //public bool LockoutEnabled { get; set; }
        //public int AccessFailedCount { get; set; }
        //public string ConcurrencyStamp { get; set; }
    }
}



