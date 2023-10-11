using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;
using System;

namespace HitchAtmApi
{
    public class HangfireDashboardAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            HttpContext httpContext = context.GetHttpContext();

            string? header = httpContext?.Request?.Headers?["Authorization"];

            if (string.IsNullOrWhiteSpace(header) == false)
            {
                AuthenticationHeaderValue authValues = AuthenticationHeaderValue.Parse(header);

                if ("Basic".Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
                {
                    string parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authValues!.Parameter));
                    var parts = parameter.Split(':');

                    if (parts.Length > 1)
                    {
                        string headerUser = parts[0];
                        string headerPassword = parts[1];

                        string user = "admin";
                        string password = "hitch.2023";

                        if ((string.IsNullOrWhiteSpace(headerUser) == false) &&
                            (string.IsNullOrWhiteSpace(headerPassword) == false))
                        {
                            return (headerUser == user && headerPassword == password) ||
                                Challenge(httpContext);
                        }
                    }
                }
            }

            return Challenge(httpContext);
        }

        private bool Challenge(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
            context.Response.WriteAsync("Authentication is required.").Wait();
            return false;
        }
    }
}
