namespace Dynamics365API.Dtos
{
    public class EmailConfirmDto
    {
        public string Email { get; set; }
        //public bool IsConfirmed { get; set; }
        public bool EmailSent { get; set; }
        public bool EmailVerified { get; set; }
    }
}
