using System.Threading.Tasks;
using System.Collections.Generic;
using HitchAtmApi.Models;

namespace HitchAtmApi.Lib
{
    public class LogService
    {
        ConnectionParameters DefaultConnectionParameters;

        public LogService(ConnectionParameters connectionParameters)
        {
            DefaultConnectionParameters = connectionParameters;
        }

        async public Task Log(LogItem logItem)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"INSERT INTO ApiLogs
                    (RequestTime, ResponseMillis, StatusCode, Method, Path, QueryString, RequestBody, ResponseBody)
                    VALUES (@RequestTime, @ResponseMillis, @StatusCode, @Method, @Path, @QueryString, @RequestBody, @ResponseBody)",
                    logItem);
            }
        }

        async public Task<List<LogItem>> GetLogs(string module, int from)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                string pattern = string.IsNullOrEmpty(module)
                    ? ""
                    : module;

                List<LogItem> logs = await provider.QueryAll<LogItem>(string.Format(
                    @"SELECT * FROM ApiLogs WHERE Path LIKE '%{0}%'
                    ORDER BY RequestTime DESC
                    LIMIT 30 OFFSET {1}",
                    pattern, from));

                return (logs == null || logs?.Count == 0)
                    ? null
                    : logs;
            }
        }
    }
}
