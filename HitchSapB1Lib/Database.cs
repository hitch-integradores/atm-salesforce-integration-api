using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using HitchSapB1Lib.Enums;
using Sap.Data.Hana;
using Dapper;

namespace HitchSapB1Lib
{
    public class Database : IDisposable
    {
        private IDbConnection Connection;
        private ConnectionParameters ConnectionParameters;

        public Database(ConnectionParameters Parameters)
        {
            ConnectionParameters = Parameters;
        }

        private string ToHanaConnectionString()
        {
            return $"Server={ConnectionParameters.LicenseServer};" +
                $"Current Schema={ConnectionParameters.DatabaseCompany};" +
                $"UserID={ConnectionParameters.DatabaseUser};" +
                $"Password={ConnectionParameters.DatabasePassword}";
        }

        private string ToSqlServerConnectionString()
        {
            return $"Server={ConnectionParameters.DatabaseServer};" +
                $"Database={ConnectionParameters.DatabaseCompany};" +
                $"User ID={ConnectionParameters.DatabaseUser};" +
                $"Password={ConnectionParameters.DatabasePassword};";
        }

        public void Connect()
        {
            try
            {
                if (ConnectionParameters == null ||
                ConnectionParameters == default(ConnectionParameters))
                {
                    throw new Exception(
                        "(501) Los parametros de conexion son invalidos");
                }

                if (Connection != null)
                {
                    if (Connection.State == ConnectionState.Open)
                    {
                        return;
                    }
                }

                Connection = ConnectionParameters.ServerType == DatabaseServerType.HANA
                    ? new HanaConnection(ToHanaConnectionString()) as IDbConnection
                    : new SqlConnection(ToSqlServerConnectionString()) as IDbConnection;

                Connection.Open();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("(501)"))
                {
                    throw ex;
                }

                throw new Exception($"(501) Error de conexion a servidor de bases de datos: {ex.Message}");
            }
        }

        public T QueryOne<T>(string Query) => Connection.QueryFirstOrDefault<T>(Query);
        public List<T> QueryAll<T>(string Query) => Connection.Query<T>(Query).ToList();

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
