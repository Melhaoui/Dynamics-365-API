using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos.Crm
{
    public class CrmTeamOpportunitiesAllEmailDto
    {
        [Display(Name = "@odata.etag")]
        public string? OdataEtag { get; set; }

        public string? Emailaddress1 { get; set; }
    }
}
