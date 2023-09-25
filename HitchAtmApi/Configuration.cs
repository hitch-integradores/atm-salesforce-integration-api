using HitchAtmApi.Lib;
using HitchSapB1Lib.Enums;

namespace HitchAtmApi
{
    public class Configuration
    {
        public ConnectionParameters PostgresConnectionParameters { get; set; }
        public SapConnectionParameters SapConnectionsParameters { get; set; }
    }

    public class SapConnectionParameters
    {
        public string Server { get; set; }
        public string ServerDatabase { get; set; }
        public string ServerUser { get; set; }
        public string ServerPassword { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string License { get; set; }
        public string DatabaseType { get; set; }

        public HitchSapB1Lib.ConnectionParameters ToConnectionParameters()
        {
            var parameters = new HitchSapB1Lib.ConnectionParameters
            {
                DatabaseCompany = ServerDatabase,
                DatabasePassword = ServerPassword,
                DatabaseServer = Server,
                DatabaseUser = ServerUser,
                LicenseServer = License,
                SapPassword = Password,
                SapUser = Username
            };

            if (DatabaseType == "MSSQL2005")
            {
                parameters.ServerType = DatabaseServerType.MSSQL2005;
            }
            else if (DatabaseType == "MSSQL2008")
            {
                parameters.ServerType = DatabaseServerType.MSSQL2008;
            }
            else if (DatabaseType == "MSSQL2012")
            {
                parameters.ServerType = DatabaseServerType.MSSQL2012;
            }
            else if (DatabaseType == "MSSQL2014")
            {
                parameters.ServerType = DatabaseServerType.MSSQL2014;
            }
            else if (DatabaseType == "MSSQL2016")
            {
                parameters.ServerType = DatabaseServerType.MSSQL2016;
            }
            else if (DatabaseType == "MSSQL2017")
            {
                parameters.ServerType = DatabaseServerType.MSSQL2017;
            }
            else if (DatabaseType == "MSSQL2019")
            {
                parameters.ServerType = DatabaseServerType.MSSQL2019;
            }
            else if (DatabaseType == "HANA")
            {
                parameters.ServerType = DatabaseServerType.HANA;
            }

            return parameters;
        }
    }
}
