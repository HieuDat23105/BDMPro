using System.ComponentModel.DataAnnotations;

namespace BDMPro.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
