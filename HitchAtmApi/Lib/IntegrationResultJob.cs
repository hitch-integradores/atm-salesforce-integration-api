using System;
using System.IO;
using System.Threading.Tasks;
using HitchAtmApi.Models;

namespace HitchAtmApi.Lib
{
    public class IntegrationResultJob
    {
        private Hs2Service _Hs2Service;

        public IntegrationResultJob(Hs2Service hs2Service)
        {
            _Hs2Service = hs2Service;
        }

        public async Task IntegrationJob(SapResponse sapResponse)
        {
            SalesforceApi salesforceApi = new SalesforceApi(
                                               Utils.Credentials, Utils.SalesforceInstanceUrl, Utils.SalesforceApiVersion);
            var credentials = salesforceApi.GetCredentials(true);
            salesforceApi.Token = credentials.Token;
            salesforceApi.UpdateCodeSapSalesForce(sapResponse.CustomerCodeSalesforce, sapResponse.CustomerCodeSap);
            await IntegrationResultAsync("BusinessAccount", sapResponse.CustomerCodeSap, sapResponse.CustomerCodeSalesforce, "CardCode");
            if (string.IsNullOrEmpty(sapResponse.ContactCodeSap?.ToString()) == false && string.IsNullOrEmpty(sapResponse.ContactCodeSalesforce?.ToString()) == false)
            {
                salesforceApi.UpdateContactSalesForce(sapResponse.ContactCodeSalesforce, sapResponse.ContactCodeSap.ToString());
            }

            if (string.IsNullOrEmpty(sapResponse.ShipToCodeSap?.ToString()) == false && string.IsNullOrEmpty(sapResponse.ShipToCodeSalesforce?.ToString()) == false)
            {
                await IntegrationResultAsync("DeliveryAddress", sapResponse.ShipToCodeSap.ToString(), sapResponse.ShipToCodeSalesforce, "ShipToCode");
            }

            if (string.IsNullOrEmpty(sapResponse.PayToCodeSap?.ToString()) == false && string.IsNullOrEmpty(sapResponse.PayToCodeSalesforce?.ToString()) == false)
            {
                await IntegrationResultAsync("DeliveryAddress", sapResponse.PayToCodeSap.ToString(), sapResponse.PayToCodeSalesforce, "PayToCode");
            }

            _Hs2Service.postApiIntegrationResult("accounts", sapResponse.CustomerCodeSap);
            if (string.IsNullOrEmpty(sapResponse.ContactCodeSap?.ToString()) == false && string.IsNullOrEmpty(sapResponse.ContactCodeSalesforce?.ToString()) == false)
            {
                await IntegrationResultAsync("Contact", sapResponse.ContactCodeSap.ToString(), sapResponse.ContactCodeSalesforce, "CNTCCode");
                _Hs2Service.postApiIntegrationResult("contacts", sapResponse.ContactCodeSap.ToString());
            }

        }

        public async Task<bool> IntegrationResultAsync(string resourceName, string resourceId, string resourceSalesforce, string typeIntegration)
        {


            IntegrationResult integrationResult = await _Hs2Service.GetIntegrationResultOne(resourceName, resourceId);

            if (integrationResult != null)
            {
                if (string.IsNullOrEmpty(integrationResult.SalesforceId))
                {
                    bool validateUpdate = await _Hs2Service.UpdateIntegrationResultSalesforceId(resourceSalesforce, integrationResult.Id.ToString());

                    if (!validateUpdate)
                    {
                        throw new Exception($"No se ha podido actualizar integrationResult con resourceId: {resourceId}");
                    }
                }
            }
            //SI NO EXISTE integrationResult SE CREA
            else
            {
                bool validateInsert = await _Hs2Service.InsertIntegrationResult(resourceName, resourceId, resourceSalesforce);
                if (!validateInsert)
                {
                    throw new Exception($"No se ha podido insertar resourceName: {resourceName}, resourceId: {resourceId}, typeIntegration: {typeIntegration} en tabla IntegrationResult");
                }
            }
            return true;
        }
    }
}