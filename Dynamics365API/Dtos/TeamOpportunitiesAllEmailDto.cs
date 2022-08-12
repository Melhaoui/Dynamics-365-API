using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos
{
    public class TeamOpportunitiesAllEmailDto
    {
        [Display(Name = "@odata.etag")]
        public string? OdataEtag { get; set; }

        public string? Emailaddress1 { get; set; }
    }
}
