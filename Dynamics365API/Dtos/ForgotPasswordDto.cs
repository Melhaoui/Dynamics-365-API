using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress, Display(Name = "Registered email address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }
}
