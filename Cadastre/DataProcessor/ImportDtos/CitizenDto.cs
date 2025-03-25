namespace Cadastre.DataProcessor.ImportDtos
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class CitizenDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        [JsonProperty(nameof(FirstName))]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        [JsonProperty(nameof(LastName))]
        public string LastName { get; set; } = null!;

        [Required]
        [JsonProperty(nameof(BirthDate))]
        public string BirthDate { get; set; } = null!;

        [Required]
        [JsonProperty(nameof(MaritalStatus))]
        public string MaritalStatus { get; set; } = null!;

        [Required]
        public int[] Properties { get; set; } = null!;
    }
}
