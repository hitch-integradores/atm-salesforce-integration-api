using System;
using HitchSapB1Lib.Objects.Inventory;

namespace HitchSapB1Lib.Operations
{
    public class UpdateTransferRequestOperation : SapOperation
    {
        public TransferRequest TransferRequest = null;
        public int DocEntry = 0;

        public void Start()
        {
            Connect();

            if (PreExecutionHook == null)
            {
                DefaultPreHook();
            }

            OperationResult Result = base.Start((object Object) =>
            {
                TransferRequest = Object as TransferRequest;

                BaseOperations BaseOperations = new BaseOperations();
                BaseOperations.Company = Company;
                BaseOperations.SapCompany = SapCompany;

                BaseOperations.UpdateTransferRequest(DocEntry, TransferRequest);

                return 1;
            });

            if (Result.Result == null)
            {
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
                    Result = TransferRequest
                };
            };
        }
    }
}
