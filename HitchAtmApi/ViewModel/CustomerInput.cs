using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HitchAtmApi.ViewModel
{
    public class CustomerInput
    {
        /// <summary>Rut del socio de negocio</summary>
        [JsonProperty("LicTradNum")]
        [Required]
        public string LicTradNum { get; set; }
    }
}
