using System.Diagnostics.CodeAnalysis;
using System.Text;
using Cadastre.Data.Enumerations;
using Cadastre.DataProcessor.ImportDtos;
using Cadastre.Utilities;
using Newtonsoft.Json;

namespace Cadastre.DataProcessor
{
    using Cadastre.Data;
    using Cadastre.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid Data!";
        private const string SuccessfullyImportedDistrict =
            "Successfully imported district - {0} with {1} properties.";
        private const string SuccessfullyImportedCitizen =
            "Succefully imported citizen - {0} {1} with {2} properties.";

        public static string ImportDistricts(CadastreContext dbContext, string xmlDocument)
        {
            StringBuilder sb = new StringBuilder();

            DistrictImportDto[]? districtDto
                = XmlHelper.Deserialize<DistrictImportDto[]>(xmlDocument, "Districts");

            var existingDistricts = dbContext
                .Districts
                .Select(d => d.Name)
                .ToArray();

            if (districtDto != null && districtDto.Length > 0)
            {
                ICollection<District> districtsToAdd = new List<District>();

                foreach (DistrictImportDto districtImportDto in districtDto)
                {
                    if (!IsValid(districtImportDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (existingDistricts.Contains(districtImportDto.Name) ||
                        districtsToAdd.Any(d => d.Name == districtImportDto.Name))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isValidRegion = Enum.TryParse<Region>(districtImportDto.Region, out Region parsedRegion);

                    if (!isValidRegion)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    ICollection<Property> propertiesToAdd = new List<Property>();

                    foreach (PropertyDto propertyDto in districtImportDto.Properties)
                    {
                        if (!IsValid(propertyDto))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        bool isValidArea = int.TryParse(propertyDto.Area, out int parsedArea);
                        bool isValidDate =
                            DateTime.TryParseExact(propertyDto.DateOfAcquisition, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out DateTime parsedDate);

                        if ((!isValidArea) || (!isValidDate))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (propertiesToAdd.Any(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier) ||
                           districtsToAdd.Any(d => d.Properties.Any(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier)) ||
                           dbContext.Properties.Any(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (propertiesToAdd.Any(p => p.Address == propertyDto.Address) ||
                            districtsToAdd.Any(d => d.Properties.Any(p => p.Address == propertyDto.Address)) ||
                            dbContext.Properties.Any(p => p.Address == propertyDto.Address))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        Property property = new Property()
                        {
                            PropertyIdentifier = propertyDto.PropertyIdentifier,
                            Area = parsedArea,
                            Details = propertyDto.Details,
                            Address = propertyDto.Address,
                            DateOfAcquisition = parsedDate
                        };

                        propertiesToAdd.Add(property);
                    }

                    District district = new District()
                    {
                        Region = parsedRegion,
                        Name = districtImportDto.Name,
                        PostalCode = districtImportDto.PostalCode,
                        Properties = propertiesToAdd.ToArray()
                    };

                    districtsToAdd.Add(district);
                    sb.AppendLine(string.Format(SuccessfullyImportedDistrict, districtImportDto.Name,
                        district.Properties.Count));
                }
                dbContext.AddRange(districtsToAdd);
                dbContext.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportCitizens(CadastreContext dbContext, string jsonDocument)
        {
            StringBuilder sb = new StringBuilder();

            CitizenDto[]? citizenDto = JsonConvert.DeserializeObject<CitizenDto[]>(jsonDocument);

            if (citizenDto != null && citizenDto.Length > 0)
            {
                ICollection<Citizen> citizensToAdd = new List<Citizen>();

                foreach (CitizenDto citizen in citizenDto)
                {
                    if (!IsValid(citizen))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isValidBirthDate = DateTime.TryParseExact(citizen.BirthDate, "dd-MM-yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedBirthDay);

                    bool isValidMaritalStatus =
                        Enum.TryParse<MaritalStatus>(citizen.MaritalStatus, out MaritalStatus parsedMaritalStatus);

                    if ((!isValidBirthDate) || (!isValidMaritalStatus))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    ICollection<PropertyCitizen> propertyToAddDtos = new List<PropertyCitizen>();

                    foreach (int propertyIdDto in citizen.Properties)
                    {
                        //bool isValidIdProp = int.TryParse(propertyIdDto.Property, out int parsedProperty);

                        //if (!isValidIdProp)
                        //{
                        //    sb.AppendLine(ErrorMessage); 
                        //    continue;
                        //}

                        if (!dbContext.Properties.Any(p=>p.Id == propertyIdDto))
                        {
                            continue;
                        }


                        PropertyCitizen property = new PropertyCitizen()
                        {
                            PropertyId = propertyIdDto
                        };

                        propertyToAddDtos.Add(property);
                    }

                    Citizen citizenEntity = new Citizen()
                    {
                        FirstName = citizen.FirstName,
                        LastName = citizen.LastName,
                        BirthDate = parsedBirthDay,
                        MaritalStatus = parsedMaritalStatus,
                        PropertiesCitizens = propertyToAddDtos.ToArray()
                    };

                    citizensToAdd.Add(citizenEntity);
                    sb.AppendLine(string.Format(SuccessfullyImportedCitizen, citizenEntity.FirstName,
                        citizenEntity.LastName, citizenEntity.PropertiesCitizens.Count));
                }
                dbContext.AddRange(citizensToAdd);
                dbContext.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
