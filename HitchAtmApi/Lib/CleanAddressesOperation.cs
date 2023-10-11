using System;
using System.Collections.Generic;
using HitchSapB1Lib;

namespace HitchAtmApi.Lib
{
    public class CleanAddressesOperation : SapOperation
    {
        public string CardCode { get; set; }

        public void Start()
        {
            Connect();

            if (PreExecutionHook == null)
            {
                DefaultPreHook();
            }

            OperationResult Result = base.Start((object _) =>
            {
                BaseOperations BaseOperations = new BaseOperations();
                BaseOperations.Company = Company;
                BaseOperations.SapCompany = SapCompany;

                List<dynamic> addressCount = null;

                try
                {
                    addressCount = BaseOperations.Company.QueryResult<dynamic>(string.Format(@"
                    SELECT COUNT(*) AS Count, Address AS Address FROM CRD1
                    WHERE CardCode = '{0}'
                    GROUP BY Address
                    HAVING COUNT(*) > 1", CardCode));
                }
                catch (Exception err)
                {
                    throw new Exception($"Error consultando direcciones repetidas. {err.Message}\n{err.StackTrace}");
                }

                if (addressCount != null && addressCount?.Count > 0)
                {
                    try
                    {

                        var Partner = BaseOperations.SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners) as SAPbobsCOM.BusinessPartners;
                        Partner.GetByKey(CardCode);

                        foreach (var address in addressCount)
                        {
                            List<dynamic> lines = BaseOperations.Company.QueryResult<dynamic>(string.Format(@"
                            SELECT LineNum FROM CRD1
                            WHERE CardCode = '{0}'
                            AND Address = '{1}'
                            ORDER BY LineNum ASC", CardCode, address.Address));

                            for (int i = 0; i < lines.Count; i++)
                            {
                                if (i == 0) continue;

                                Partner.Addresses.SetCurrentLine(Convert.ToInt32(lines[i].LineNum));
                                Partner.Addresses.Delete();
                            }
                        }

                        Partner.Update();
                    }
                    catch (Exception err)
                    {
                        throw new Exception($"Error eliminando direcciones repetidas. {err.Message}\n{err.StackTrace}");
                    }
                }


                return new OperationResult
                {
                    Result = 1,
                    Exception = null
                };
            });

            if (Result.Exception != null)
            {
                throw Result.Exception;
            }
        }

        private void DefaultPreHook()
        {
            PreExecutionHook = () =>
            {
                return new HookResult
                {
                    Exception = null,
                    Result = 1
                };
            };
        }
    }
}
