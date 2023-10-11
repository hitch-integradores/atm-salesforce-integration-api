using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HitchAtmApi.ViewModel
{
    public class ProductInput
    {
        /// <summary>Codigo del articulo</summary>
        [JsonProperty("ItemCode")]
        [Required]
        public string ItemCode { get; set; }
    }
}
