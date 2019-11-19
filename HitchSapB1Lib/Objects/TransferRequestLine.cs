using System;
using System.Collections.Generic;
namespace HitchSapB1Lib.Objects.Inventory
{
    public class TransferRequestLine
    {
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public string FromWarehouseCode { get; set; }
        public string ToWarehouseCode { get; set; }
        public DocumentReference Reference { get; set; }
        public List<Batch> Batchs { get; set; }
        public List<Serial> Series { get; set; }
        public List<UserField> UserFields { get; set; }
    }
}