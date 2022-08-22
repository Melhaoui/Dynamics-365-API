using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos
{
    public class ProfileDto
    {
        [Required, StringLength(100)]
        public string Firstname { get; set; }

        [Required, StringLength(100)]
        public string lastname { get; set; }

        [Required, StringLength(100)]
        public string Jobtitle { get; set; }

        [Required, StringLength(150)]
        public string emailaddress1 { get; set; }

        [Required, StringLength(100)]
        public string Telephone1 { get; set; }

        [Required, StringLength(100)]
        public string Mobilephone { get; set; }

        public string? Fax { get; set; }

        [Required]
        public int PreferredcontactMethodCode { get; set; }

        public string? Address1_line1 { get; set; }
        
        public string? Address1_line2 { get; set; }

        public string? Address1_line3 { get; set; }

        public string? Address1_city { get; set; }

        public string? Address1_stateorprovince { get; set; }

        public string? Address1_postalcode { get; set; }

        public string? Address1_country { get; set; }
    }
}
