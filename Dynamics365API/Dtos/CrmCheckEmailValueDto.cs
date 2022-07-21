using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos
{
    public class CrmCheckEmailValueDto
    {
        [Display(Name = "@odata.etag")]
        public string? odataEtag { get; set; }

        public string? fullname { get; set; }

        public string? emailaddress1 { get; set; }

        public string? contactid { get; set; }
    }
}
