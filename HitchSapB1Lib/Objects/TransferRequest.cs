using System;
using System.Collections.Generic;

namespace HitchSapB1Lib.Objects.Inventory
{
    public class TransferRequest
    {
        public string CustomerCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? StartDeliveryDate { get; set; }
        public DateTime? EndDeliveryDate { get; set; }
        public int? SalesEmployeeCode { get; set; }
        public int? ContactCode { get; set; }
        public int? Serie { get; set; }
        public string Comment { get; set; }
        public string JournalMemo { get; set; }
        public string ShipToCode { get; set; }
        public int? PriceListCode { get; set; }
        public string FromWarehouseCode { get; set; }
        public string ToWarehouseCode { get; set; }
        public List<TransferRequestLine> Lines { get; set; }
        public List<UserField> UserFields { get; set; }
        public List<string> Attachments { get; set; }
    }
}