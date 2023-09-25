using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HitchAtmApi.Lib
{
    public class SalesforceResponse
    {
        public int Status { get; set; }
        public string Content { get; set; }
    }

    public class Credentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string GrantType { get; set; }
    }

    public class AccessToken
    {
        public string Token { get; set; }
        public string InstanceUrl { get; set; }
        public string Id { get; set; }
        public string TokenType { get; set; }
        public string IssuedAt { get; set; }
        public string Signature { get; set; }
    }

    public class SalesforceApi
    {
        private Credentials Credentials;
        private string BaseUrl;
        private string Version;
        public string Token;

        public SalesforceApi(Credentials credentials, string baseUrl, string version)
        {
            Credentials = credentials;
            BaseUrl = baseUrl;
            Version = version;
        }

        public AccessToken GetToken()
        {
            var client = new RestClient(BaseUrl);
            string url = "services/oauth2/token";

            RestRequest request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter(
                "application/x-www-form-urlencoded",
                $"grant_type={Credentials.GrantType}&client_secret={Credentials.ClientSecret}" +
                $"&username={Credentials.Username}&password={Credentials.Password}{Credentials.Token}" +
                $"&client_id={Credentials.ClientId}",
                ParameterType.RequestBody);

            RestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"No es posible obtener el token de acceso usando las credenciales del archivo de configuracion. Respuesta de salesforce {response.Content}");
            }

            JObject responseParsed = JsonConvert.DeserializeObject<JObject>(response.Content);

            AccessToken accessToken = new AccessToken
            {
                Token = responseParsed["access_token"].ToObject<string>(),
                InstanceUrl = responseParsed["instance_url"].ToObject<string>(),
                Id = responseParsed["id"].ToObject<string>(),
                TokenType = responseParsed["token_type"].ToObject<string>(),
                IssuedAt = responseParsed["issued_at"].ToObject<string>(),
                Signature = responseParsed["signature"].ToObject<string>()
            };

            File.WriteAllText(Path.Combine(Program.AUTH_PATH, "credentials.json"), JsonConvert.SerializeObject(accessToken));

            return accessToken;
        }

        public dynamic GetAccount(string Code)
        {
            SalesforceResponse response = GetRequest(
                $"services/data/{Version}/sobjects/Account/Codigo_SAP__c/{Code}");

            if (response.Status == 403)
            {
                throw new Exception("Se ha excedido el limite de request de Salesforce");
            }

            if (response.Status == 404)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }

        public dynamic GetDeliveryAddress(string Code)
        {
            SalesforceResponse response = GetRequest(
                $"services/data/{Version}/sobjects/Direccion_de_despacho__c/id_externo__c/{Code}");

            if (response.Status == 403)
            {
                throw new Exception("Se ha excedido el limite de request de Salesforce");
            }

            if (response.Status == 404)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }

        public void UpdateDeliveryAddress(string id, string newCode)
        {
            SalesforceResponse response = PatchRequest($"services/data/{Version}/sobjects/Direccion_de_despacho__c/{id}", new
            {
                id_externo__c = newCode
            });

            if (response.Status == 403)
            {
                throw new Exception("Se ha excedido el limite de request de Salesforce");
            }

            if (response.Status != 204)
            {
                throw new Exception($"No fue posible actualizar el id externo de la direccion en salesforce. Error: {response.Content}");
            }
        }

        public SalesforceResponse GetRequest(string url)
        {
            var client = new RestClient(BaseUrl);
            RestRequest request = new RestRequest(url, Method.Get);
            request.AddHeader("Content-Type", "application/json");

            if (string.IsNullOrEmpty(Token) == false)
            {
                request.AddHeader("Authorization", $"Bearer {Token}");
            }

            RestResponse response = client.Execute(request);

            return new SalesforceResponse
            {
                Status = (int)response.StatusCode,
                Content = response.Content
            };
        }

        public SalesforceResponse PostRequest(string url, object body)
        {
            var client = new RestClient(BaseUrl);
            RestRequest request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");

            if (string.IsNullOrEmpty(Token) == false)
            {
                request.AddHeader("Authorization", $"Bearer {Token}");
            }

            request.AddJsonBody(body);

            RestResponse response = client.Execute(request);

            return new SalesforceResponse
            {
                Status = (int)response.StatusCode,
                Content = response.Content
            };
        }

        public SalesforceResponse GetDeliveryAddressAllData(string code)
        {
            return PostRequest(
                $"", new
                {
                    allOrNone = false,
                    compositeRequest = new object[]
                    {
                        new
                        {
                            method = "GET",
                            referenceId = "Address",
                            url = $"/services/data/v45.0/sobjects/Direccion_de_despacho__c/id_externo__c/{code}"
                        },
                        new
                        {
                            method = "GET",
                            referenceId = "Comuna",
                            url = "/services/data/v45.0/sobjects/Comuna__c/@{Address.Comuna__c}"
                        },
                        new
                        {
                            method = "GET",
                            referenceId = "Region",
                            url = "/services/data/v45.0/sobjects/Region__c/@{Address.Region__c}"
                        },
                        new
                        {
                            method = "GET",
                            referenceId = "Pais",
                            url = "/services/data/v45.0/sobjects/Pais__c/@{Address.Pais__c}"
                        }
                    }
                });
        }

        public SalesforceResponse PatchRequest(string url, object body)
        {
            var client = new RestClient(BaseUrl);
            RestRequest request = new RestRequest(url, Method.Patch);
            request.AddHeader("Content-Type", "application/json");

            if (string.IsNullOrEmpty(Token) == false)
            {
                request.AddHeader("Authorization", $"Bearer {Token}");
            }

            request.AddJsonBody(body);

            RestResponse response = client.Execute(request);

            return new SalesforceResponse
            {
                Status = (int)response.StatusCode,
                Content = response.Content
            };
        }
    }
}
