using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace HitchAtmApi.Models
{
    public class HttpResponse
    {
        public class Response<T>
        {
            [JsonProperty("Message")]
            public string Message { get; set; }
            [JsonProperty("Error")]
            public string Error { get; set; }
            [JsonProperty("Data")]
            public T Data { get; set; }
            [JsonIgnore]
            public int Status { get; set; }

            public string DefaultMessage(int Status)
            {
                return ReasonPhrases.GetReasonPhrase(Status);
            }

            public JsonResult Send()
            {
                JsonResult response = new JsonResult(this);
                response.StatusCode = Status;
                return response;
            }
        }

        public class Error : Response<object>
        {
            public Error(int status, string message, string error)
            {
                Status = status;
                Message = message;
                Error = error ?? DefaultMessage(status);
            }
        }
    }
}
