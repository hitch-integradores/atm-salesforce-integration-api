using Newtonsoft.Json;

namespace HitchAtmApi.Models
{
    public class Customer
    {
        /// <summary>Nombre del socio de negocio</summary>
        [JsonProperty("CardName")]
        public string CardName { get; set; }

        /// <summary>Nombre extranjero del socio de negocio</summary>
        [JsonProperty("CardFName")]
        public string CardFName { get; set; }

        /// <summary>Rut del socio de negocio</summary>
        [JsonProperty("LicTradNum")]
        public string LicTradNum { get; set; }

        /// <summary>Codigo del socio de negocio</summary>
        [JsonProperty("CardCode")]
        public string CardCode { get; set; }
    }
}
