using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos
{
    public class ProfileDto
    {
        [Required, StringLength(100)]
        public string firstname { get; set; }

        [Required, StringLength(100)]
        public string lastname { get; set; }

        [Required, StringLength(100)]
        public string jobtitle { get; set; }

        [Required, StringLength(150)]
        public string emailaddress1 { get; set; }

        [Required, StringLength(100)]
        public string telephone1 { get; set; }

        [Required, StringLength(100)]
        public string mobilephone { get; set; }

        public string? fax { get; set; }

        [Required]
        public int preferredcontactmethodcode { get; set; }

        public string? entityimage { get; set; }
        public string? address1_line1 { get; set; }
        
        public string? address1_line2 { get; set; }
                       
        public string? address1_line3 { get; set; }
                       
        public string? address1_city { get; set; }
                       
        public string? address1_stateorprovince { get; set; }
                       
        public string? address1_postalcode { get; set; }
                       
        public string? address1_country { get; set; }
    }
}
