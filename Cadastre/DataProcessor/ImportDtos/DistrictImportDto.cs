using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ImportDtos
{
    using Cadastre.Data.Enumerations;
    using System.ComponentModel.DataAnnotations;
    [XmlType("District")]
    public class DistrictImportDto
    {
        [XmlElement(nameof(Name))]
        [Required]
        [MinLength(2)]
        [MaxLength(80)]
        public string Name { get; set; } = null!;

        [XmlElement(nameof(PostalCode))]
        [Required]
        [RegularExpression(@"[A-Z]{2}-\d{5}")]
        public string PostalCode { get; set; } = null!;

        [XmlAttribute(nameof(Region))]
        [Required]
        public string Region { get; set; } = null!;

        [XmlArray(nameof(Properties))]
        public PropertyDto[] Properties { get; set; } = null!;
    }
}
