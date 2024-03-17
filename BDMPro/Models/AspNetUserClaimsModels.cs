using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BDMPro.Models
{
    public class AspNetUserClaims: IdentityUserClaim<string>
    {
        //[Key]
        //public int Id { get; set; }
        //[MaxLength(450)]
        //public string UserId { get; set; }
        //public string ClaimType { get; set; }
        //public string ClaimValue { get; set; }

    }
}
