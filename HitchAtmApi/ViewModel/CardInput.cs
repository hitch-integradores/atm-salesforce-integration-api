using HitchAtmApi.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HitchAtmApi.ViewModel
{
    public class CardInput
    {
        /// <summary>Codigo de la tarjeta de equipo</summary>
        [JsonProperty("insID")]
        [Required]
        public int insID { get; set; }

        /// <summary>Estado de la tarjeta de equipo</summary>
        [JsonProperty("Status")]
        public string Status { get; set; }

        /// <summary>Datos de la direccion</summary>
        [JsonProperty("Address")]
        public Address Address { get; set; }
    }
}
