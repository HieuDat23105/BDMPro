using System.ComponentModel.DataAnnotations;

namespace BDMPro.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
