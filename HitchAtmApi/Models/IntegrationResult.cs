using System;

namespace HitchAtmApi.Models
{
    public class IntegrationResult
    {
        public string? Id { get; set; }
        public string? ResourceName { get; set; }
        public string? ResourceId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreateBody { get; set; }
        public string? UpdateBody { get; set; }
        public string? Error { get; set; }
        public string? SalesforceId { get; set; }
        public string? ResourceField1 { get; set; }
        public string? ResourceField2 { get; set; }
        public string? ResourceField3 { get; set; }
        public string? ResourceField4 { get; set; }
    }
}
