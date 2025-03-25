using Cadastre.Data;
using System.Globalization;
using Cadastre.Data.Enumerations;
using Cadastre.DataProcessor.ExportDtos;
using Cadastre.Utilities;
using Newtonsoft.Json;

namespace Cadastre.DataProcessor
{
    public class Serializer
    {
        public static string ExportPropertiesWithOwners(CadastreContext dbContext)
        {
            var properties = dbContext
                .Properties
                .Where(p => p.DateOfAcquisition >=
                            DateTime.ParseExact("01/01/2000", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(p => p.DateOfAcquisition)
                .ThenBy(p => p.PropertyIdentifier)
                .Select(p => new
                {
                    p.PropertyIdentifier,
                    p.Area,
                    p.Address,
                    DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture),
                    Owners = p.PropertiesCitizens.Select(c => new
                        {
                            LastName = c.Citizen.LastName,
                            MaritalStatus = c.Citizen.MaritalStatus.ToString(),
                        })
                        .OrderBy(c => c.LastName)
                        .ToArray()
                }).ToArray();

            string result = JsonConvert.SerializeObject(properties,Formatting.Indented);

            return result;
        }

        public static string ExportFilteredPropertiesWithDistrict(CadastreContext dbContext)
        {
            var filteredProperties = dbContext
                .Properties
                .Where(p => p.Area >= 100)
                .OrderByDescending(p => p.Area)
                .ThenBy(p => p.DateOfAcquisition)
                .Select(p => new PropertyExportDto()
                {
                    PostalCode = p.District.PostalCode,
                    PropertyIdentifier = p.PropertyIdentifier,
                    Area = p.Area,
                    DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                })
                .ToArray();

            string result = XmlHelper.Serialize(filteredProperties, "Properties");
            return result;
        }
    }
}
