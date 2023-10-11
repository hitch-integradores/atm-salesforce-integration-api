using System;
using HitchSapB1Lib.Objects.Marketing;

namespace HitchSapB1Lib.Operations
{
    public class AddContactToBusinessPartner : SapOperation
    {
        public Contact Contact = null;
        public string CardCode = null;
        public int? ContactCode = null;
        public string ContactName = null;

        public void Start()
        {
            Connect();

            if (PreExecutionHook == null)
            {
                DefaultPreHook();
            }

            OperationResult Result = base.Start((object Object) =>
            {
                Contact = Object as Contact;

                BaseOperations BaseOperations = new BaseOperations();
                BaseOperations.Company = Company;
                BaseOperations.SapCompany = SapCompany;

                return BaseOperations.AddContactToBusinessPartner(CardCode, Contact);
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

            Tuple<int, string> ContactInfo = Result.Result as Tuple<int, string>;
            ContactCode = ContactInfo.Item1;
            ContactName = ContactInfo.Item2;
        }

        private void DefaultPreHook()
        {
            PreExecutionHook = () =>
            {
                return new HookResult
                {
                    Exception = null,
                    Result = Contact
                };
            };
        }
    }
}
