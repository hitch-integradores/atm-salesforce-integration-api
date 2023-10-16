using Newtonsoft.Json;

namespace HitchAtmApi.Models
{
    public class SapResponse
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }

        public string ContactCodeSalesforce { get; set; }

        public int? ContactCodeSap { get; set; }
        public string CustomerCodeSap { get; set; } //CardCode
        public string CustomerCodeSalesforce { get; set; }
        public string ShipToCodeSap { get; set; }
        public string ShipToCodeSalesforce { get; set; }
        public string PayToCodeSap { get; set; }
        public string PayToCodeSalesforce { get; set; }
    }
}
