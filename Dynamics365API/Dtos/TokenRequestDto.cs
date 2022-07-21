using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos
{
    public class TokenRequestDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}