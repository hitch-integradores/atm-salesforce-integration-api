using System;
using HitchSapB1Lib.Objects.Definition;

namespace HitchSapB1Lib.Operations
{
    public class CreateProjectOperation : SapOperation
    {
        public Project Project = null;

        public void Start()
        {
            Connect();

            if (PreExecutionHook == null)
            {
                DefaultPreHook();
            }

            OperationResult Result = base.Start((object Object) =>
            {
                Project = Object as Project;

                BaseOperations BaseOperations = new BaseOperations();
                BaseOperations.Company = Company;
                BaseOperations.SapCompany = SapCompany;

                BaseOperations.CreateProject(Project);

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
                    Result = Project
                };
            };
        }
    }
}
