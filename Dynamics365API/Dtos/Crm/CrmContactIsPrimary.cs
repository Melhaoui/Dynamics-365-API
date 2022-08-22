using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dynamics365API.Dtos.Crm
{
    public class CrmContactIsPrimary
    {
        [Display(Name = "@odata.context")]
        public string odataContext { get; set; }
        public List<object>? value { get; set; }
    }
}
