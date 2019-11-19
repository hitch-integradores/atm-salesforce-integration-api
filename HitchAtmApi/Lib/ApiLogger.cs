using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using HitchAtmApi.Models;

namespace HitchAtmApi.Lib
{
    public class ApiLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private LogService _logService;

        public ApiLoggerMiddleware(
            RequestDelegate next,
            LogService logService)
        {
            _next = next;
            _logService = logService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var stopWatch = Stopwatch.StartNew();
            var requestTime = DateTime.UtcNow;
            var requestBodyContent = await ReadRequestBody(request);
            var originalBodyStream = httpContext.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                var response = httpContext.Response;
                response.Body = responseBody;
                await _next(httpContext);
                stopWatch.Stop();

                string responseBodyContent = null;
                responseBodyContent = await ReadResponseBody(response);
                await responseBody.CopyToAsync(originalBodyStream);

                await SafeLog(requestTime,
                    stopWatch.ElapsedMilliseconds,
                    response.StatusCode,
                    request.Method,
                    request.Path,
                    request.QueryString.ToString(),
                    requestBodyContent,
                    responseBodyContent);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(Microsoft.AspNetCore.Http.HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task SafeLog(DateTime requestTime,
            long responseMillis,
            int statusCode,
            string method,
            string path,
            string queryString,
            string requestBody,
            string responseBody)
        {
            await _logService.Log(new LogItem
            {
                RequestTime = requestTime,
                ResponseMillis = responseMillis,
                StatusCode = statusCode,
                Method = method,
                Path = path,
                QueryString = queryString,
                RequestBody = requestBody,
                ResponseBody = responseBody
            });
        }
    }
}
