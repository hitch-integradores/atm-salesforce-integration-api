using System;
using System.Collections.Generic;
using HitchSapB1Lib.Objects.Inventory;

namespace HitchSapB1Lib.Objects.Marketing
{
    public class DocumentLine
    {
        public string ItemCode { get; set; }
        public string TaxCode { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public string CurrencyCode { get; set; }
        public string Warehouse { get; set; }
        public int? EmployeeCode { get; set; }
        public string CostingCode1 { get; set; }
        public string CostingCode2 { get; set; }
        public string CostingCode3 { get; set; }
        public string CostingCode4 { get; set; }
        public string CostingCode5 { get; set; }
        public string Project { get; set; }
        public DocumentReference Reference { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public bool? ApplyCommission { get; set; }
        public List<Batch> Batchs { get; set; }
        public List<Serial> Series { get; set; }
        public List<UserField> UserFields { get; set; }
    }
}