using System;
using HitchSapB1Lib.Objects.Definition;

namespace HitchSapB1Lib.Operations
{
    public class AddAddressToBusinessPartner : SapOperation
    {
        public Address Address = null;
        public string CardCode = null;
        public string AddressCode = null;

        public void Start()
        {
            Connect();

            if (PreExecutionHook == null)
            {
                DefaultPreHook();
            }

            OperationResult Result = base.Start((object Object) =>
            {
                Address = Object as Address;

                BaseOperations BaseOperations = new BaseOperations();
                BaseOperations.Company = Company;
                BaseOperations.SapCompany = SapCompany;

                string Code = BaseOperations.AddAddressToBusinessPartner(CardCode, Address);

                return Code;
            });

            if (Result.PreExecutionResult.Exception != null)
            {
                throw new Exception($"No logro ejecutarse la tarea de pre-ejecucion:\n{Result.PreExecutionResult.Exception.Message}\n{Result.PreExecutionResult.Exception.StackTrace}");
            }

            if (PostExecutionHook != null)
            {
                if (Result.PostExecutionResult.Exception != null)
                {
                    throw new Exception($"No logro ejecutarse la tarea de post-ejecucion:\n{Result.PostExecutionResult.Exception.Message}\n{Result.PostExecutionResult.Exception.StackTrace}");
                }
            }

            if (Result.Exception != null)
            {
                throw Result.Exception;
            }

            AddressCode = Result.Result as string;
        }

        private void DefaultPreHook()
        {
            PreExecutionHook = () =>
            {
                return new HookResult
                {
                    Exception = null,
                    Result = Address
                };
            };
        }
    }
}
