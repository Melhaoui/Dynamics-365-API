using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos.Crm
{
    public class CrmTeamOpportunitiesDataDto<T>
    {
        [Display(Name = "@odata.context")]
        public string OdataContext { get; set; }

        public List<T>? Value { get; set; }
    }
}
