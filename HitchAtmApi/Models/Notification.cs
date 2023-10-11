using System;

namespace HitchAtmApi.Models
{
    public class Notification
    {
        public long Id { get; set; }
        public long RefId { get; set; }
        public string RefType { get; set; }
        public string Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public int Steps { get; set; }
        public string Stage { get; set; }
        public int? DocNum { get; set; }
        public int? DocEntry { get; set; }
    }
}
