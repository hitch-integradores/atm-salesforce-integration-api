using HitchSapB1Lib.Enums;

namespace HitchSapB1Lib
{
    public class ConnectionParameters
    {
        public string DatabaseServer { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabaseCompany { get; set; }
        public string SapUser { get; set; }
        public string SapPassword { get; set; }
        public string LicenseServer { get; set; }
        public DatabaseServerType ServerType { get; set; }
    }
}
