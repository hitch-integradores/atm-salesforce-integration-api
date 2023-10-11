using System;
using System.Collections.Generic;
using HitchSapB1Lib.Enums;

namespace HitchSapB1Lib
{
    public partial class Company : IDisposable
    {
        private Database Database = null;
        public ConnectionParameters ConnectionParameters = null;

        public Company(ConnectionParameters Parameters)
        {
            ConnectionParameters = Parameters;
        }

        public T QueryOneResult<T>(string Query)
        {
            try
            {
                if (Database == null)
                {
                    Database = new Database(ConnectionParameters);
                    Database.Connect();
                }

                return Database.QueryOne<T>(Query);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("(501)"))
                {
                    throw ex;
                }

                throw new Exception($"(501) Error de comunicacion con la base de datos: {ex.Message}");
            }
        }

        public List<T> QueryResult<T>(string Query)
        {
            try
            {
                if (Database == null)
                {
                    Database = new Database(ConnectionParameters);
                    Database.Connect();
                }

                return Database.QueryAll<T>(Query);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("(501)"))
                {
                    throw ex;
                }

                throw new Exception($"(501) Error de comunicacion con la base de datos: {ex.Message}");
            }
        }

        public bool IsHana
        {
            get
            {
                return ConnectionParameters.ServerType == DatabaseServerType.HANA;
            }
        }

        public void Dispose()
        {
            if (Database != null)
            {
                Database.Dispose();
                Database = null;
            }
        }
    }
}
