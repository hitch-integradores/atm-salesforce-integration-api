using System;

namespace HitchAtmApi.Models
{
    public class NotificationLog
    {
        public long Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Status { get; set; }
        public string Operation { get; set; }
        public string Message { get; set; }
        public long NotificationId { get; set; }
    }
}
