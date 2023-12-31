﻿using System;
using HitchSapB1Lib.Objects.Inventory;

namespace HitchSapB1Lib.Operations
{
    public class CreateTransferRequestOperation : SapOperation
    {
        public TransferRequest TransferRequest = null;
        public int? DocNum = null;
        public int? DocEntry = null;

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

                int NewKey = BaseOperations.CreateTransferRequest(TransferRequest);

                int Num = Company.QueryOneResult<int>(Company.IsHana
                    ? $"SELECT T0.\"DocNum\" FROM OWTQ T0 WHERE T0.\"DocEntry\" = {NewKey}"
                    : $"SELECT T0.DocNum FROM OWTQ T0 WHERE T0.DocEntry = {NewKey}");

                return new Tuple<int, int>(NewKey, Num);
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

            var ResultNumbers = Result.Result as Tuple<int, int>;
            DocEntry = ResultNumbers.Item1;
            DocNum = ResultNumbers.Item2;
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
