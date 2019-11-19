using System;
using HitchSapB1Lib.Objects.Services;

namespace HitchSapB1Lib.Operations
{
    public class UpdateEquipmentCardParams
    {
        public int CardId { get; set; }
        public EquipmentCard EquipmentCard { get; set; }
    }

    public class UpdateEquipmentCardOperation : SapOperation
    {
        public UpdateEquipmentCardParams UpdateEquipmentCardParams = null;

        public void Start()
        {
            Connect();

            if (PreExecutionHook == null)
            {
                DefaultPreHook();
            }

            OperationResult Result = base.Start((object Object) =>
            {
                UpdateEquipmentCardParams = Object as UpdateEquipmentCardParams;

                BaseOperations BaseOperations = new BaseOperations();
                BaseOperations.Company = Company;
                BaseOperations.SapCompany = SapCompany;

                BaseOperations.UpdateEquipmentCard(UpdateEquipmentCardParams.CardId, UpdateEquipmentCardParams.EquipmentCard);

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
                    Result = UpdateEquipmentCardParams
                };
            };
        }
    }
}
