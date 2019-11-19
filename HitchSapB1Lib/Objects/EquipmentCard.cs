using HitchSapB1Lib.Enums;
using HitchSapB1Lib.Objects.Definition;

namespace HitchSapB1Lib.Objects.Services
{
    public class EquipmentCard
    {
        public EquipmentCardType? CardType { get; set; }
        public string SerialNumberManufacturer { get; set; }
        public string SerialNumber { get; set; }
        public string ItemCode { get; set; }
        public EquipmentCardStatus? CardStatus { get; set; }
        public string CustomerCode { get; set; }
        public int? ContactCard { get; set; }
        public int? DefaultTechnical { get; set; }
        public int? DefaultTerritory { get; set; }
        public Address Address { get; set; }

    }
}
