using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDMPro.Models
{
    public class SupplierContact : IdentityUserRole<string>
    {
        [Key, Column(Order = 1)]
        [MaxLength(128)]
        public string UserId { get; set; }
        //[Key, Column(Order = 2)]
        //[MaxLength(450)]
        //public string RoleId { get; set; }
    }
}