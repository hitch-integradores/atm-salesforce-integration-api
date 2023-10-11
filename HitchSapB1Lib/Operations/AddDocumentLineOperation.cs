using System;
using System.Collections.Generic;
using HitchSapB1Lib.Objects.Marketing;

namespace HitchSapB1Lib.Operations
{
    public class AddDocumentLineParams
    {
        public int DocumentDocEntry { get; set; }
        public int ObjectType { get; set; }
        public List<DocumentLine> Lines { get; set; }
    }

    public class AddDocumentLineOperation : SapOperation
    {
        AddDocumentLineParams AddDocumentLineParams = null;

        public void Start()
        {
            Connect();

            if (PreExecutionHook == null)
            {
                DefaultPreHook();
            }

            OperationResult Result = base.Start((object Object) =>
            {
                AddDocumentLineParams = Object as AddDocumentLineParams;

                BaseOperations BaseOperations = new BaseOperations();
                BaseOperations.Company = Company;
                BaseOperations.SapCompany = SapCompany;

                BaseOperations.AddDocumentLines(AddDocumentLineParams.DocumentDocEntry, AddDocumentLineParams.Lines, AddDocumentLineParams.ObjectType);

                return null;
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
        }

        private void DefaultPreHook()
        {
            PreExecutionHook = () =>
            {
                return new HookResult
                {
                    Exception = null,
                    Result = AddDocumentLineParams
                };
            };
        }
    }
}
