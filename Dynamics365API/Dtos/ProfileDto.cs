using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos
{
    public class ProfileDto
    {
        [Required]
        [DataType(DataType.Date)]
        public string? birthdate { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string? mobilephone { get; set; }

        [Required, MaxLength(20)]
        public string lastname { get; set; }
        [Required, MaxLength(20)]
        public string firstname { get; set; }
        [Required]
        public int? gendercode { get; set; }
    }
}
