using System;
using System.Threading.Tasks;
using HitchAtmApi.Models;

namespace HitchAtmApi.Lib
{
    public class IntegrationResultJob
    {
        private Hs2Service _Hs2Service;
        private readonly SalesforceApi _SalesforceApi;
        public IntegrationResultJob(Hs2Service hs2Service, SalesforceApi salesforceApiNew)
        {
            _Hs2Service = hs2Service;
            _SalesforceApi = salesforceApiNew;
        }

        public async Task IntegrationJob(string CardCode, int CNTCCode, string ShipToCode, string PayToCode)
        {
            await IntegrationResultAsync("BusinessAccount", CardCode, "CardCode");
            await IntegrationResultAsync("Contact", CNTCCode.ToString(), "CNTCCode");
            await IntegrationResultAsync("DeliveryAddress", ShipToCode.ToString(), "ShipToCode");
            await IntegrationResultAsync("DeliveryAddress", PayToCode.ToString(), "PayToCode");
        }

        public async Task<bool> IntegrationResultAsync(string resourceName, string resourceId, string typeIntegration)
        {
            IntegrationResult integrationResult = await _Hs2Service.GetIntegrationResultOne(resourceName, resourceId);
            //SI EXISTE integrationResult SE ACTUALIZA
            if (integrationResult != null)
            {
                if (string.IsNullOrEmpty(integrationResult.SalesforceId.Trim()))
                {
                    if (typeIntegration == "CardCode" || typeIntegration == "ShipToCode" || typeIntegration == "PayToCode")
                    {
                        var responseSalesForce = typeIntegration == "CardCode" ? _SalesforceApi.GetAccount(resourceId) : _SalesforceApi.GetDeliveryAddress(resourceId);
                        if (responseSalesForce != null && responseSalesForce.Id != null)
                        {
                            bool validateUpdate = await _Hs2Service.UpdateIntegrationResultSalesforceId(responseSalesForce.Id.ToString(), integrationResult.Id.ToString());

                            if (validateUpdate)
                            {
                                _Hs2Service.postApiIntegrationResult("accounts", resourceId);
                            }
                            else
                            {
                                throw new Exception($"No se ha podido actualizar integrationResult con resourceId: {resourceId}");
                            }
                        }
                    }

                    else if (typeIntegration == "CNTCCode")
                    {
                        var responseSalesForce = _SalesforceApi.GetContact(resourceId);
                        if (responseSalesForce != null && responseSalesForce.records != null && responseSalesForce.records.Count > 0)
                        {
                            bool validateUpdate = await _Hs2Service.UpdateIntegrationResultSalesforceId(responseSalesForce.records[0].Id.ToString(), integrationResult.Id.ToString());

                            if (validateUpdate)
                            {
                                _Hs2Service.postApiIntegrationResult("contacts", resourceId);
                            }
                            else
                            {
                                throw new Exception($"No se ha podido actualizar integrationResult con resourceId: {resourceId}");
                            }
                        }
                    }
                }
            }
            //SI NO EXISTE integrationResult SE CREA
            else
            {

                if (typeIntegration == "CardCode" || typeIntegration == "ShipToCode" || typeIntegration == "PayToCode")
                {
                    var responseSalesForce = typeIntegration == "CardCode" ? _SalesforceApi.GetAccount(resourceId) : _SalesforceApi.GetDeliveryAddress(resourceId);
                    if (responseSalesForce != null && responseSalesForce.Id != null)
                    {
                        bool validateInsert = await _Hs2Service.InsertIntegrationResult(resourceName, resourceId, responseSalesForce.Id.ToString());

                        if (validateInsert)
                        {

                            _Hs2Service.postApiIntegrationResult("accounts", resourceId);
                        }
                        else
                        {
                            throw new Exception($"No se ha podido insertar resourceName: {resourceName}, resourceId: {resourceId}, typeIntegration: {typeIntegration} en tabla IntegrationResult");
                        }

                    }
                }

                else if (typeIntegration == "CNTCCode")
                {
                    var responseSalesForce = _SalesforceApi.GetContact(resourceId);
                    if (responseSalesForce != null && responseSalesForce.records != null && responseSalesForce.records.Count > 0)
                    {
                        bool validateInsert = await _Hs2Service.InsertIntegrationResult(resourceName, resourceId, responseSalesForce.records[0].Id.ToString());
                        if (validateInsert)
                        {
                            _Hs2Service.postApiIntegrationResult("contacts", resourceId);
                        }
                        else
                        {
                            throw new Exception($"No se ha podido insertar resourceName: {resourceName}, resourceId: {resourceId}, typeIntegration: {typeIntegration} en tabla IntegrationResult");
                        }
                    }
                }
            }
            return true;
        }
    }
}