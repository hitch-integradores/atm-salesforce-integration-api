using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Npgsql;
using Dapper;

namespace HitchAtmApi.Lib
{
    public class Database : IDisposable
    {
        private IDbConnection Connection;

        public Database(ConnectionParameters connectionParameters)
        {
            ConnectDatabase(connectionParameters);
        }

        private void ConnectDatabase(ConnectionParameters connectionParameters)
        {
            if (connectionParameters == null ||
                connectionParameters == default(ConnectionParameters))
            {
                throw new Exception(
                    "Invalid database connection params");
            }

            if (Connection != null && Connection?.State == ConnectionState.Open)
            {
                Connection.Dispose();
            }

            Connection = new NpgsqlConnection(connectionParameters.ToConnectionString());
        }

        async public Task<T> QueryOne<T>(
            string Query,
            object Parameters = null,
            ConnectionParameters connectionParameters = null)
        {
            if (connectionParameters != null)
            {
                ConnectDatabase(connectionParameters);
            }

            if (Parameters != null)
            {
                return await Connection.QueryFirstOrDefaultAsync<T>(Query, Parameters);
            }

            return await Connection.QueryFirstOrDefaultAsync<T>(Query);
        }

        async public Task<List<T>> QueryAll<T>(string Query,
            object Parameters = null,
            ConnectionParameters connectionParameters = null)
        {
            if (connectionParameters != null)
            {
                ConnectDatabase(connectionParameters);
            }

            if (Parameters != null)
            {
                return (await Connection.QueryAsync<T>(Query, Parameters)).ToList();
            }

            return (await Connection.QueryAsync<T>(Query)).ToList();
        }

        async public Task ExecCommand(
            string Command,
            object Parameters = null,
            ConnectionParameters connectionParameters = null,
            bool IsProcedure = false)
        {
            if (connectionParameters != null)
            {
                ConnectDatabase(connectionParameters);
            }

            if (Parameters != null)
            {
                await Connection.ExecuteAsync(Command, Parameters,
                    commandType: IsProcedure
                        ? CommandType.StoredProcedure
                        : CommandType.Text);
                return;
            }

            await Connection.ExecuteAsync(Command,
                commandType: IsProcedure
                    ? CommandType.StoredProcedure
                    : CommandType.Text);
        }

        async public Task ExecCommands<T>(
            string Command,
            List<T> Parameters,
            ConnectionParameters connectionParameters = null,
            bool IsProcedure = false)
        {
            if (connectionParameters != null)
            {
                ConnectDatabase(connectionParameters);
            }

            await Connection.ExecuteAsync(Command, Parameters,
                commandType: IsProcedure
                    ? CommandType.StoredProcedure
                    : CommandType.Text);
            return;
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }

    public class ConnectionParameters
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public string ToConnectionString()
        {
            return $"Host={Server};" +
                $"Database={Database};" +
                $"User ID={User};" +
                $"Password={Password};";
        }
    }
}
