using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HitchAtmApi.Models
{
    public class Address
    {
        /// <summary>ID de la direccion</summary>
        [JsonProperty("Street")]
        [Required]
        public string Street { get; set; }

        /// <summary>Codigo del pais de la direccion</summary>
        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }

        /// <summary>Ciudad de la direccion</summary>
        [JsonProperty("City")]
        public string City { get; set; }

        /// <summary>Comuna de la direccion</summary>
        [JsonProperty("County")]
        public string County { get; set; }

        /// <summary>Codigo de la region de la direccion</summary>
        [JsonProperty("StateCode")]
        public string StateCode { get; set; }

        /// <summary>Bloque</summary>
        [JsonProperty("Block")]
        public string Block { get; set; }

        /// <summary>Codigo postal</summary>
        [JsonProperty("Zip")]
        public string Zip { get; set; }

        /// <summary>Edificio/Sala/Planta</summary>
        [JsonProperty("Building")]
        public string Building { get; set; }

        /// <summary>Numero de la calle de la direccion</summary>
        [JsonProperty("StreetNumber")]
        public string StreetNumber { get; set; }
    }
}
