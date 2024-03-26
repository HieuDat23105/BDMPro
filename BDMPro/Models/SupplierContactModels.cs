using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDMPro.Models
{
    public class SupplierContact
    {
        [Key, Column(Order = 1)]
        [MaxLength(128)]
        public string SupplierId { get; set; }
        [Key, Column(Order = 2)]
        [MaxLength(128)]
        public string ContactId { get; set; }
    }
}