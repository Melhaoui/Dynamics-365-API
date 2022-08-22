using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos.Crm
{
    public class CrmCheckEmailDataDto
    {
        [Display(Name = "@odata.context")]
        public string odataContext { get; set; }

        [Display(Name = "@Microsoft.Dynamics.CRM.totalrecordcount")]
        public int? crmTotalrecordcount { get; set; }


        [Display(Name = "@Microsoft.Dynamics.CRM.totalrecordcountlimitexceeded")]
        public bool? crmtotalrecordcountlimitexceeded { get; set; }

        [Display(Name = "Microsoft.Dynamics.CRM.globalmetadataversion")]
        public string? crmGlobalmetadataV { get; set; }


        public List<CrmCheckEmailValueDto>? value { get; set; }
    }
}
