using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDMPro.Models
{
    public class AspNetUserLogins:IdentityUserLogin<string>
    {
        //[Key, Column(Order = 1)]
        //[MaxLength(128)]
        //public string LoginProvider { get; set; }
        //[Key, Column(Order = 2)]
        //[MaxLength(128)]
        //public string ProviderKey { get; set; }
        //[Key, Column(Order = 3)]
        //[MaxLength(128)]
        //public string UserId { get; set; }
        //public string ProviderDisplayName { get; set; }
    }
}