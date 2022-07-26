using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }

        //public string? Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}
