using System;
using HitchSapB1Lib.Enums;
using System.Collections.Generic;

namespace HitchSapB1Lib.Objects.Marketing
{
    public class BusinessPartner
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardFName { get; set; }
        public string LicTradNum { get; set; }
        public string Phone { get; set; }
        public string OtherPhone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string CurrencyIsoCode { get; set; }
        public string Comments { get; set; }
        public bool? IsActive { get; set; }
        public int? GroupCode { get; set; }
        public int? PaymentCondition { get; set; }
        public double? CreditLine { get; set; }
        public int? PriceListNum { get; set; }
        public string ProjectCode { get; set; }
        public List<UserField> UserFields { get; set; }
    }
}
