using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using HitchAtmApi.Models;
using RestSharp;

namespace HitchAtmApi.Lib
{
    public class Hs2Service
    {
        string _sqlServerConnectionString; // Cadena de conexión de SQL Server
        string _urlApi;

        public Hs2Service(
            string sqlServerConnectionString,
            string urlApi)
        {
            _sqlServerConnectionString = sqlServerConnectionString;
            _urlApi = urlApi;
        }
        
        async public Task<IntegrationResult> GetIntegrationResultOne(string resourceName, string resourceId)
        {
            using (SqlConnection connection = new SqlConnection(_sqlServerConnectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(
                    "SELECT * FROM IntegrationResult WHERE ResourceId = @ResourceId AND ResourceName = @ResourceName", connection))
                {
                    command.Parameters.AddWithValue("@ResourceId", resourceId);
                    command.Parameters.AddWithValue("@ResourceName", resourceName);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        // Verifica si se encontraron resultados
                        if (await reader.ReadAsync())
                        {
                            IntegrationResult result = new IntegrationResult
                            {
                                Id = reader["Id"] != DBNull.Value ? reader["Id"].ToString() : null,
                                ResourceName = reader["ResourceName"] != DBNull.Value ? reader["ResourceName"].ToString() : null,
                                ResourceId = reader["ResourceId"] != DBNull.Value ? reader["ResourceId"].ToString() : null,
                                Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : null,
                                CreatedDate = reader["CreatedDate"] != DBNull.Value ? (DateTime?)reader["CreatedDate"] : null,
                                UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? (DateTime?)reader["UpdatedDate"] : null,
                                CreateBody = reader["CreateBody"] != DBNull.Value ? reader["CreateBody"].ToString() : null,
                                UpdateBody = reader["UpdateBody"] != DBNull.Value ? reader["UpdateBody"].ToString() : null,
                                Error = reader["Error"] != DBNull.Value ? reader["Error"].ToString() : null,
                                SalesforceId = reader["SalesforceId"] != DBNull.Value ? reader["SalesforceId"].ToString() : null,
                                ResourceField1 = reader["ResourceField1"] != DBNull.Value ? reader["ResourceField1"].ToString() : null,
                                ResourceField2 = reader["ResourceField2"] != DBNull.Value ? reader["ResourceField2"].ToString() : null,
                                ResourceField3 = reader["ResourceField3"] != DBNull.Value ? reader["ResourceField3"].ToString() : null,
                                ResourceField4 = reader["ResourceField4"] != DBNull.Value ? reader["ResourceField4"].ToString() : null
                            };
                            return result;
                        }
                        return null; // No se encontró ningún resultado
                    }
                }
            }
        }


        async public Task<bool> UpdateIntegrationResultSalesforceId(string salesforceId, string id)
        {
            using (SqlConnection connection = new SqlConnection(_sqlServerConnectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(
                    "UPDATE IntegrationResult SET SalesforceId = @SalesforceId, UpdatedDate = @UpdatedDate, Status = @Status WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@SalesforceId", salesforceId);
                    command.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@Status", "Pendient");
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        // throw new InvalidOperationException("No se encontró ningún registro para actualizar.");
                        return false;
                    }

                    return true;
                }
            }
        }


        async public Task<bool> InsertIntegrationResult(string resourceName, string resourceId, string SalesforceId)
        {
            using (SqlConnection connection = new SqlConnection(_sqlServerConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO IntegrationResult (ResourceName, ResourceId, Status, CreatedDate, UpdatedDate,  CreateBody, UpdateBody, Error, SalesforceId, ResourceField1, ResourceField2, ResourceField3, ResourceField4) " +
                    "VALUES (@ResourceName, @ResourceId, @Status, @CreatedDate, @UpdatedDate, @CreateBody, @UpdateBody, @Error, @SalesforceId, @ResourceField1, @ResourceField2, @ResourceField3, @ResourceField4)", connection))
                {
                    command.Parameters.AddWithValue("@ResourceName", resourceName);
                    command.Parameters.AddWithValue("@ResourceId", resourceId);
                    command.Parameters.AddWithValue("@Status", "Pendient");
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@CreateBody", "");
                    command.Parameters.AddWithValue("@UpdateBody", "");
                    command.Parameters.AddWithValue("@Error", "");
                    command.Parameters.AddWithValue("@SalesforceId", SalesforceId);
                    command.Parameters.AddWithValue("@ResourceField1", "");
                    command.Parameters.AddWithValue("@ResourceField2", "");
                    command.Parameters.AddWithValue("@ResourceField3", "");
                    command.Parameters.AddWithValue("@ResourceField4", "");

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }


        async public Task<string> GetTokenSession()
        {
            using (SqlConnection connection = new SqlConnection(_sqlServerConnectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(
                    "SELECT TOP 1 Token FROM TokenSession ORDER BY Id DESC", connection))
                {
                    var result = await command.ExecuteScalarAsync();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                    return null; // No se encontró token
                }
            }
        }

        async public Task<bool> InsertTokenSession(AccessToken tokenSession)
        {
            using (SqlConnection connection = new SqlConnection(_sqlServerConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO TokenSession (Token, InstanceUrl, ResponseId, TokenType, IssuedAt, Signature,  CreatedDate) " +
                    "VALUES (@Token, @InstanceUrl, @ResponseId, @TokenType, @IssuedAt, @Signature, @CreatedDate)", connection))
                {
                    command.Parameters.AddWithValue("@Token", tokenSession.Token);
                    command.Parameters.AddWithValue("@InstanceUrl", tokenSession.InstanceUrl);
                    command.Parameters.AddWithValue("@ResponseId", tokenSession.Id);
                    command.Parameters.AddWithValue("@TokenType", tokenSession.TokenType);
                    command.Parameters.AddWithValue("@IssuedAt", tokenSession.IssuedAt);
                    command.Parameters.AddWithValue("@Signature", tokenSession.Signature);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }

        public SalesforceResponse postApiIntegrationResult(string action, string resourceId)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new Exception("action no enviada en postApiIntegrationResult");
            }
            
            if (string.IsNullOrEmpty(resourceId))
            {
                throw new Exception("resourceId no enviado en postApiIntegrationResult");
            }
            
            var client = new RestClient(_urlApi);
            string url = $"jobs/{action}/individual";
            RestRequest request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                Id = resourceId
            });

            RestResponse response = client.Execute(request);

            return new SalesforceResponse
            {
                Status = (int)response.StatusCode,
                Content = response.Content
            };
        }
    }
}
