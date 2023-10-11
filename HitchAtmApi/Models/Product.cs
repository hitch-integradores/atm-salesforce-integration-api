using Newtonsoft.Json;

namespace HitchAtmApi.Models
{
    public class Product
    {
        [JsonProperty("ItemCode")]
        public string ItemCode { get; set; }

        [JsonProperty("ItemName")]
        public string ItemName { get; set; }

        [JsonProperty("StatusProduct")]
        public int StatusProduct { get; set; }
    }
}
