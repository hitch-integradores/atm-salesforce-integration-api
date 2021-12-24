using HitchSapB1Lib.Enums;

namespace HitchSapB1Lib.Objects.Definition
{
    public class Address
    {
        public string AddressCode { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Street { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string StateCode { get; set; }
        public string Block { get; set; }
        public string Zip { get; set; }
        public string Building { get; set; }
        public string StreetNumber { get; set; }
        public string TaxCode { get; set; }
        public string NumGlobalLocation { get; set; }
        public AddressType Type { get; set; }
    }
}
