using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos.Crm
{
    public class CrmTeamOpportunitiesValueDto
    {
        [Display(Name = "@odata.etag")]
        public string? OdataEtag { get; set; }

        public string? ContactId { get; set; }

        public string? AccountId { get; set; }

    }
}
