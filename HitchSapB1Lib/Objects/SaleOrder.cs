using System;
using System.Collections.Generic;
using HitchSapB1Lib.Enums;

namespace HitchSapB1Lib.Objects.Marketing
{
    public class SaleOrder
    {
        public string CustomerCode { get; set; }
        public string CustomerReferenceNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime DueDate { get; set; }
        public int? SalesEmployeeCode { get; set; }
        public int? OwnerCode { get; set; }
        public int? ContactCode { get; set; }
        public double? Discount { get; set; }
        public int? Serie { get; set; }
        public string Comment { get; set; }
        public string ShipToCode { get; set; }
        public string PayToCode { get; set; }
        public bool IsPartialDelivery { get; set; }
        public CurrencySource? CurrencySource { get; set; }
        public List<DocumentLine> Lines { get; set; }
        public List<UserField> UserFields { get; set; }
    }
}