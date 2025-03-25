using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ExportDtos
{
    [XmlType("Property")]
    public class PropertyExportDto
    {
        [XmlElement(nameof(PropertyIdentifier))]
        public string PropertyIdentifier { get; set; } = null!;
        [XmlElement(nameof(Area))]
        public int Area { get; set; }

        [XmlElement(nameof(DateOfAcquisition))]
        public string DateOfAcquisition { get; set; } = null!;

        [XmlAttribute("postal-code")]
        public string PostalCode { get; set; } = null!;
    }
}
