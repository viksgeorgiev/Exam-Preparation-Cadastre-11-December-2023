using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    [XmlType("Property")]
    public class PropertyDto
    {
        [XmlElement(nameof(PropertyIdentifier))]
        [Required]
        [MinLength(16)]
        [MaxLength(20)]
        public string PropertyIdentifier { get; set; } = null!;

        [XmlElement(nameof(Area))] 
        [Required] 
        public string Area { get; set; } = null!;

        [XmlElement(nameof(Details))]
        [MinLength(5)]
        [MaxLength(500)]
        public string? Details { get; set; }

        [XmlElement(nameof(Address))]
        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        public string Address { get; set; } = null!;

        [XmlElement(nameof(DateOfAcquisition))]
        [Required]
        public string DateOfAcquisition { get; set; } = null!;
    }
}