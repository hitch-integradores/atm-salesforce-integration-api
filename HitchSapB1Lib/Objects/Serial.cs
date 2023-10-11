using System;

namespace HitchSapB1Lib.Objects.Inventory
{
    public class Serial
    {
        public string Number { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
