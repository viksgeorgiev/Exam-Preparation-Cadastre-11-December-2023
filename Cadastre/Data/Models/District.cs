using Cadastre.Data.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Cadastre.Data.Models
{
    public class District
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(80)]
        public string Name { get; set; } = null!;

        [Required]
        [Column(TypeName = "VARCHAR(8)")]
        [RegularExpression(@"[A-Z]{2}-\d{5}")]
        public string PostalCode { get; set; } = null!;

        [Required]
        public Region Region { get; set; }

        public virtual ICollection<Property> Properties { get; set; } 
            = new HashSet<Property>();
    }
}
