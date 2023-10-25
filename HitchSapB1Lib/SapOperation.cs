using System;
using System.IO;
using System.Collections.Generic;
using Polly;
using HitchSapB1Lib.Enums;
using HitchSapB1Lib.Objects;
using HitchSapB1Lib.Objects.Definition;
using HitchSapB1Lib.Objects.Marketing;
using HitchSapB1Lib.Objects.Inventory;
using HitchSapB1Lib.Objects.Shopping;
using HitchSapB1Lib.Objects.Services;

namespace HitchSapB1Lib
{
    public class SapOperation
    {
        protected SAPbobsCOM.Company SapCompany = null;
        public Company Company = null;
        public Func<HookResult> PreExecutionHook = null;
        public Func<object, HookResult> PostExecutionHook = null;

        public Dictionary<DatabaseServerType, SAPbobsCOM.BoDataServerTypes> ServerTypes = new Dictionary<DatabaseServerType, SAPbobsCOM.BoDataServerTypes>
        {
            { DatabaseServerType.MSSQL2005, SAPbobsCOM.BoDataServerTypes.dst_MSSQL2005 },
            { DatabaseServerType.MSSQL2008, SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008 },
            { DatabaseServerType.MSSQL2012, SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012 },
            { DatabaseServerType.MSSQL2014, SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014 },
            { DatabaseServerType.MSSQL2016, SAPbobsCOM.BoDataServerTypes.dst_MSSQL2016 },
            { DatabaseServerType.MSSQL2017, SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017 },
            { DatabaseServerType.HANA, SAPbobsCOM.BoDataServerTypes.dst_HANADB }
        };

        protected OperationResult Start(Func<object, object> Operation)
        {
            OperationResult Result = new OperationResult();
            Result.PostExecutionResult = null;
            Result.PreExecutionResult = null;

            if (PreExecutionHook != null)
            {
                HookResult PreExecutionResult = ExecuteHook(() =>
                {
                    return PreExecutionHook();
                });

                if (PreExecutionResult.Exception != null)
                {
                    Result.PreExecutionResult = PreExecutionResult;
                    Result.Result = null;
                    Result.PostExecutionResult = null;

                    return Result;
                }

                Result.PreExecutionResult = PreExecutionResult;
            }

            try
            {
                object OperationExecutionResult = Operation(Result.PreExecutionResult.Result);
                Result.Result = OperationExecutionResult;
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
                return Result;
            }

            if (PostExecutionHook != null)
            {
                HookResult PostExecutionResult = ExecuteHook(() =>
                {
                    return PostExecutionHook(Result.Result);
                });

                Result.PostExecutionResult = PostExecutionResult;
            }

            return Result;
        }

        protected HookResult ExecuteHook(Func<HookResult> action)
        {
            var policyResult = Policy<HookResult>
                .Handle<Exception>()
                .Fallback(x => DefaultResult())
                .ExecuteAndCapture(() =>
                {
                    return action();
                });

            if (policyResult.FinalException != null)
            {
                return new HookResult
                {
                    Exception = policyResult.FinalException,
                    Result = null
                };
            }

            return policyResult.Result;
        }

        protected HookResult DefaultResult()
        {
            return new HookResult
            {
                Exception = null,
                Result = null
            };
        }

        protected void Connect()
        {
            try
            {
                if (SapCompany == null)
                {
                    SapCompany = new SAPbobsCOM.Company();
                }

                if (!SapCompany.Connected)
                {
                    SapCompany.Server = Company.ConnectionParameters.DatabaseServer;
                    SapCompany.LicenseServer = Company.ConnectionParameters.LicenseServer;
                    SapCompany.language = SAPbobsCOM.BoSuppLangs.ln_Spanish;
                    SapCompany.DbServerType = ServerTypes[Company.ConnectionParameters.ServerType];
                    SapCompany.DbUserName = Company.ConnectionParameters.DatabaseUser;
                    SapCompany.DbPassword = Company.ConnectionParameters.DatabasePassword;
                    SapCompany.CompanyDB = Company.ConnectionParameters.DatabaseCompany;
                    SapCompany.UserName = Company.ConnectionParameters.SapUser;
                    SapCompany.Password = Company.ConnectionParameters.SapPassword;

                    if (SapCompany.Connect() != 0)
                    {
                        throw new Exception($"(501) Error de conexion a SAP: {SapCompany.GetLastErrorCode()} {SapCompany.GetLastErrorDescription()}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("(501)"))
                {
                    throw ex;
                }

                throw new Exception($"(501) Error de conexion a SAP: {ex.Message}");
            }
        }

        protected class BaseOperations
        {
            public Company Company = null;
            public SAPbobsCOM.Company SapCompany = null;

            public int CreateSaleOrder(SaleOrder Order)
            {
                try
                {
                    SAPbobsCOM.Documents Document = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders) as SAPbobsCOM.Documents;
                    SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                    Document.CardCode = Order.CustomerCode;
                    Document.DocDate = Order.DocumentDate;
                    Document.TaxDate = Order.TaxDate;
                    Document.DocDueDate = Order.DueDate;
                    Document.PartialSupply = Order.IsPartialDelivery
                        ? SAPbobsCOM.BoYesNoEnum.tYES
                        : SAPbobsCOM.BoYesNoEnum.tNO;

                    if (Order.PaymentCondition.HasValue)
                    {
                        Document.PaymentGroupCode = Order.PaymentCondition.Value;
                    }

                    if (string.IsNullOrEmpty(Order.ShipToCode) == false)
                    {
                        Document.ShipToCode = Order.ShipToCode;
                    }
                    if (string.IsNullOrEmpty(Order.PayToCode) == false)
                    {
                        Document.PayToCode = Order.PayToCode;
                    }
                    if (Order.SalesEmployeeCode.HasValue)
                    {
                        Document.SalesPersonCode = Order.SalesEmployeeCode.Value;
                    }

                    if (Order.ContactCode.HasValue)
                    {
                        Document.ContactPersonCode = Order.ContactCode.Value;
                    }

                    if (Order.Discount.HasValue)
                    {
                        Document.DiscountPercent = Order.Discount.Value;
                    }

                    if (string.IsNullOrEmpty(Order.Comment) == false)
                    {
                        Document.Comments = Order.Comment;
                    }

                    if (string.IsNullOrEmpty(Order.CustomerReferenceNumber) == false)
                    {
                        Document.NumAtCard = Order.CustomerReferenceNumber;
                    }

                    if (Order.OwnerCode.HasValue)
                    {
                        Document.DocumentsOwner = Order.OwnerCode.Value;
                    }

                    if (Order.Serie.HasValue)
                    {
                        Document.Series = Order.Serie.Value;
                    }

                    if (Order.Lines == null || Order.Lines?.Count == 0)
                    {
                        throw new Exception("La orden de venta debe tener una o mas lineas");
                    }

                    AddDocumentLines(Order.Lines, ref Document, ref SapRecordSet);

                    if (Order.UserFields != null && Order.UserFields?.Count > 0)
                    {
                        foreach (UserField field in Order.UserFields)
                        {
                            if (field.Value == null)
                            {
                                Document.UserFields.Fields.Item(field.Name).SetNullValue();
                                continue;
                            }
                            Document.UserFields.Fields.Item(field.Name).Value = field.Value;
                        }
                    }

                    if (Document.Add() != 0)
                    {
                        throw new Exception(
                            $"({SapCompany.GetLastErrorCode()}) Error registrando orden de venta: {SapCompany.GetLastErrorDescription()}");
                    }

                    return Convert.ToInt32(SapCompany.GetNewObjectKey());
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error registrando orden de venta: {ex.Message}");
                }
            }

            public int UpdateSaleOrder(int DocEntry, SaleOrder Order)
            {
                try
                {
                    SAPbobsCOM.Documents Document = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders) as SAPbobsCOM.Documents;
                    SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                    Document.GetByKey(DocEntry);

                    Document.PartialSupply = Order.IsPartialDelivery
                        ? SAPbobsCOM.BoYesNoEnum.tYES
                        : SAPbobsCOM.BoYesNoEnum.tNO;

                    if (string.IsNullOrEmpty(Order.ShipToCode) == false)
                    {
                        Document.ShipToCode = Order.ShipToCode;
                    }
                    if (string.IsNullOrEmpty(Order.PayToCode) == false)
                    {
                        Document.PayToCode = Order.PayToCode;
                    }
                    if (Order.SalesEmployeeCode.HasValue)
                    {
                        Document.SalesPersonCode = Order.SalesEmployeeCode.Value;
                    }

                    if (Order.ContactCode.HasValue)
                    {
                        Document.ContactPersonCode = Order.ContactCode.Value;
                    }

                    if (Order.Discount.HasValue)
                    {
                        Document.DiscountPercent = Order.Discount.Value;
                    }

                    if (string.IsNullOrEmpty(Order.Comment) == false)
                    {
                        Document.Comments = Order.Comment;
                    }

                    if (string.IsNullOrEmpty(Order.CustomerReferenceNumber) == false)
                    {
                        Document.NumAtCard = Order.CustomerReferenceNumber;
                    }

                    if (Order.OwnerCode.HasValue)
                    {
                        Document.DocumentsOwner = Order.OwnerCode.Value;
                    }

                    if (Order.Serie.HasValue)
                    {
                        Document.Series = Order.Serie.Value;
                    }

                    if (Order.UserFields != null && Order.UserFields?.Count > 0)
                    {
                        foreach (UserField field in Order.UserFields)
                        {
                            if (field.Value == null)
                            {
                                Document.UserFields.Fields.Item(field.Name).SetNullValue();
                                continue;
                            }
                            Document.UserFields.Fields.Item(field.Name).Value = field.Value;
                        }
                    }

                    if (Document.Update() != 0)
                    {
                        throw new Exception(
                            $"({SapCompany.GetLastErrorCode()}) Error actualizando orden de venta: {SapCompany.GetLastErrorDescription()}");
                    }

                    return Convert.ToInt32(SapCompany.GetNewObjectKey());
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error actualizando orden de venta: {ex.Message}");
                }
            }

            public int CreatePurchaseOrder(PurchaseOrder Order)
            {
                try
                {
                    SAPbobsCOM.Documents Document = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                    SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                    Document.CardCode = Order.ProviderCode;
                    Document.DocDate = Order.DocumentDate;
                    Document.TaxDate = Order.TaxDate;
                    Document.DocDueDate = Order.DueDate;

                    if (string.IsNullOrEmpty(Order.ShipToCode) == false)
                    {
                        Document.ShipToCode = Order.ShipToCode;
                    }
                    if (string.IsNullOrEmpty(Order.PayToCode) == false)
                    {
                        Document.PayToCode = Order.PayToCode;
                    }
                    if (Order.BuyEmployeeCode.HasValue)
                    {
                        Document.SalesPersonCode = Order.BuyEmployeeCode.Value;
                    }
                    if (Order.ContactCode.HasValue)
                    {
                        Document.ContactPersonCode = Order.ContactCode.Value;
                    }
                    if (Order.Discount.HasValue)
                    {
                        Document.DiscountPercent = Order.Discount.Value;
                    }

                    if (string.IsNullOrEmpty(Order.Comment) == false)
                    {
                        Document.Comments = Order.Comment;
                    }

                    if (string.IsNullOrEmpty(Order.ProviderReferenceNumber) == false)
                    {
                        Document.NumAtCard = Order.ProviderReferenceNumber;
                    }

                    if (Order.OwnerCode.HasValue)
                    {
                        Document.DocumentsOwner = Order.OwnerCode.Value;
                    }

                    if (Order.Serie.HasValue)
                    {
                        Document.Series = Order.Serie.Value;
                    }

                    if (Order.Lines == null || Order.Lines?.Count == 0)
                    {
                        throw new Exception("La orden de compra debe tener una o mas lineas");
                    }

                    AddDocumentLines(Order.Lines, ref Document, ref SapRecordSet);

                    if (Order.UserFields != null && Order.UserFields?.Count > 0)
                    {
                        foreach (UserField field in Order.UserFields)
                        {
                            if (field.Value == null)
                            {
                                Document.UserFields.Fields.Item(field.Name).SetNullValue();
                                continue;
                            }
                            Document.UserFields.Fields.Item(field.Name).Value = field.Value;
                        }
                    }

                    if (Document.Add() != 0)
                    {
                        throw new Exception(
                            $"({SapCompany.GetLastErrorCode()}) Error registrando orden de compra: {SapCompany.GetLastErrorDescription()}");
                    }

                    return Convert.ToInt32(SapCompany.GetNewObjectKey());
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error registrando orden de compra: {ex.Message}");
                }
            }

            public void UpdatePurchaseOrder(int DocEntry, PurchaseOrder Order)
            {
                try
                {
                    SAPbobsCOM.Documents Document = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                    SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                    Document.GetByKey(DocEntry);

                    if (string.IsNullOrEmpty(Order.ShipToCode) == false)
                    {
                        Document.ShipToCode = Order.ShipToCode;
                    }
                    if (string.IsNullOrEmpty(Order.PayToCode) == false)
                    {
                        Document.PayToCode = Order.PayToCode;
                    }
                    if (Order.BuyEmployeeCode.HasValue)
                    {
                        Document.SalesPersonCode = Order.BuyEmployeeCode.Value;
                    }
                    if (Order.ContactCode.HasValue)
                    {
                        Document.ContactPersonCode = Order.ContactCode.Value;
                    }
                    if (Order.Discount.HasValue)
                    {
                        Document.DiscountPercent = Order.Discount.Value;
                    }

                    if (string.IsNullOrEmpty(Order.Comment) == false)
                    {
                        Document.Comments = Order.Comment;
                    }

                    if (string.IsNullOrEmpty(Order.ProviderReferenceNumber) == false)
                    {
                        Document.NumAtCard = Order.ProviderReferenceNumber;
                    }

                    if (Order.OwnerCode.HasValue)
                    {
                        Document.DocumentsOwner = Order.OwnerCode.Value;
                    }

                    if (Order.Serie.HasValue)
                    {
                        Document.Series = Order.Serie.Value;
                    }

                    if (Order.UserFields != null && Order.UserFields?.Count > 0)
                    {
                        foreach (UserField field in Order.UserFields)
                        {
                            if (field.Value == null)
                            {
                                Document.UserFields.Fields.Item(field.Name).SetNullValue();
                                continue;
                            }
                            Document.UserFields.Fields.Item(field.Name).Value = field.Value;
                        }
                    }

                    if (Document.Update() != 0)
                    {
                        throw new Exception(
                            $"({SapCompany.GetLastErrorCode()}) Error actualizando orden de compra: {SapCompany.GetLastErrorDescription()}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error actualizando orden de compra: {ex.Message}");
                }
            }

            public int CreateTransferRequest(TransferRequest Transfer)
            {
                try
                {
                    SAPbobsCOM.StockTransfer Document = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest) as SAPbobsCOM.StockTransfer;
                    SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                    Document.CardCode = Transfer.CustomerCode;
                    Document.FromWarehouse = Transfer.FromWarehouseCode;
                    Document.ToWarehouse = Transfer.ToWarehouseCode;
                    Document.DocDate = Transfer.DocumentDate;
                    Document.TaxDate = Transfer.TaxDate;
                    Document.DueDate = Transfer.DueDate;

                    if (Transfer.StartDeliveryDate.HasValue)
                    {
                        Document.StartDeliveryDate = Transfer.StartDeliveryDate.Value;
                    }
                    if (Transfer.EndDeliveryDate.HasValue)
                    {
                        Document.EndDeliveryDate = Transfer.EndDeliveryDate.Value;
                    }

                    if (string.IsNullOrEmpty(Transfer.ShipToCode) == false)
                    {
                        Document.ShipToCode = Transfer.ShipToCode;
                    }
                    if (Transfer.ContactCode.HasValue)
                    {
                        Document.ContactPerson = Transfer.ContactCode.Value;
                    }
                    if (Transfer.SalesEmployeeCode.HasValue)
                    {
                        Document.SalesPersonCode = Transfer.SalesEmployeeCode.Value;
                    }
                    if (Transfer.Serie.HasValue)
                    {
                        Document.Series = Transfer.Serie.Value;
                    }
                    if (Transfer.PriceListCode.HasValue)
                    {
                        Document.PriceList = Transfer.PriceListCode.Value;
                    }
                    if (string.IsNullOrEmpty(Transfer.JournalMemo) == false)
                    {
                        Document.JournalMemo = Transfer.JournalMemo;
                    }
                    if (string.IsNullOrEmpty(Transfer.Comment) == false)
                    {
                        Document.Comments = Transfer.Comment;
                    }

                    AddDocumentLines(Transfer.Lines, ref Document, ref SapRecordSet);

                    if (Transfer.UserFields != null && Transfer.UserFields?.Count > 0)
                    {
                        foreach (UserField field in Transfer.UserFields)
                        {
                            if (field.Value == null)
                            {
                                Document.UserFields.Fields.Item(field.Name).SetNullValue();
                                continue;
                            }
                            Document.UserFields.Fields.Item(field.Name).Value = field.Value;
                        }
                    }

                    if (Transfer.Attachments != null && Transfer.Attachments?.Count > 0)
                    {
                        foreach (string attachmentFile in Transfer.Attachments)
                        {
                            int attachmentEntry = AddAttachment(attachmentFile);
                            Document.AttachmentEntry = attachmentEntry;
                        }
                    }

                    if (Document.Add() != 0)
                    {
                        throw new Exception(
                            $"({SapCompany.GetLastErrorCode()}) Error registrando solicitud de traslado: {SapCompany.GetLastErrorDescription()}");
                    }

                    return Convert.ToInt32(SapCompany.GetNewObjectKey());
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error registrando solicitud de traslado: {ex.Message}");
                }
            }

            public void UpdateTransferRequest(int DocEntry, TransferRequest Transfer)
            {
                try
                {
                    SAPbobsCOM.StockTransfer Document = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest) as SAPbobsCOM.StockTransfer;
                    SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                    Document.GetByKey(DocEntry);

                    Document.FromWarehouse = Transfer.FromWarehouseCode;
                    Document.ToWarehouse = Transfer.ToWarehouseCode;

                    if (Transfer.StartDeliveryDate.HasValue)
                    {
                        Document.StartDeliveryDate = Transfer.StartDeliveryDate.Value;
                    }
                    if (Transfer.EndDeliveryDate.HasValue)
                    {
                        Document.EndDeliveryDate = Transfer.EndDeliveryDate.Value;
                    }

                    if (string.IsNullOrEmpty(Transfer.ShipToCode) == false)
                    {
                        Document.ShipToCode = Transfer.ShipToCode;
                    }
                    if (Transfer.ContactCode.HasValue)
                    {
                        Document.ContactPerson = Transfer.ContactCode.Value;
                    }
                    if (Transfer.SalesEmployeeCode.HasValue)
                    {
                        Document.SalesPersonCode = Transfer.SalesEmployeeCode.Value;
                    }
                    if (Transfer.Serie.HasValue)
                    {
                        Document.Series = Transfer.Serie.Value;
                    }
                    if (Transfer.PriceListCode.HasValue)
                    {
                        Document.PriceList = Transfer.PriceListCode.Value;
                    }
                    if (string.IsNullOrEmpty(Transfer.JournalMemo) == false)
                    {
                        Document.JournalMemo = Transfer.JournalMemo;
                    }
                    if (string.IsNullOrEmpty(Transfer.Comment) == false)
                    {
                        Document.Comments = Transfer.Comment;
                    }

                    if (Transfer.UserFields != null && Transfer.UserFields?.Count > 0)
                    {
                        foreach (UserField field in Transfer.UserFields)
                        {
                            if (field.Value == null)
                            {
                                Document.UserFields.Fields.Item(field.Name).SetNullValue();
                                continue;
                            }
                            Document.UserFields.Fields.Item(field.Name).Value = field.Value;
                        }
                    }

                    if (Transfer.Attachments != null && Transfer.Attachments?.Count > 0)
                    {
                        foreach (string attachmentFile in Transfer.Attachments)
                        {
                            int attachmentEntry = AddAttachment(attachmentFile);
                            Document.AttachmentEntry = attachmentEntry;
                        }
                    }

                    if (Document.Update() != 0)
                    {
                        throw new Exception(
                            $"({SapCompany.GetLastErrorCode()}) Error actualizando solicitud de traslado: {SapCompany.GetLastErrorDescription()}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error actualizando solicitud de traslado: {ex.Message}");
                }
            }

            public void AddDocumentLines(int DocEntry, List<DocumentLine> Lines, int ObjectType)
            {
                SAPbobsCOM.Documents Document = null;

                if (ObjectType == 17)
                {
                    Document = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders) as SAPbobsCOM.Documents;
                }
                else if (ObjectType == 22)
                {
                    Document = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                }

                SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                if (!Document.GetByKey(DocEntry))
                {
                    throw new Exception($"No existe el documento con el DocEntry {DocEntry}");
                }

                AddDocumentLines(Lines, ref Document, ref SapRecordSet);
            }

            public void AddDocumentLines(int DocEntry, List<TransferRequestLine> Lines)
            {
                SAPbobsCOM.StockTransfer Document = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest) as SAPbobsCOM.StockTransfer;
                SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                if (!Document.GetByKey(DocEntry))
                {
                    throw new Exception($"No existe el documento con el DocEntry {DocEntry}");
                }

                AddDocumentLines(Lines, ref Document, ref SapRecordSet);
            }

            private void AddDocumentLines(List<DocumentLine> Lines, ref SAPbobsCOM.Documents Document, ref SAPbobsCOM.Recordset SapRecordSet)
            {
                bool WithPreviousLines = Document.Lines.Count > 1;

                foreach (DocumentLine line in Lines)
                {
                    if (WithPreviousLines)
                    {
                        Document.Lines.Add();
                    }

                    if (line.Reference != null)
                    {
                        LineReference Reference = GetLineReference(line.ItemCode, line.Reference, ref SapRecordSet);

                        if (Reference.Status == DocumentStatus.Closed)
                        {
                            throw new Exception($"El documento base de la referencia para la linea del articulo {line.ItemCode} se encuentra cerrado");
                        }

                        Document.Lines.BaseEntry = line.Reference.Number;
                        Document.Lines.BaseType = (int)line.Reference.Type;
                        Document.Lines.BaseLine = Reference.LineNum;
                    }

                    Document.Lines.ItemCode = line.ItemCode;
                    Document.Lines.Quantity = line.Quantity;

                    if (line.Price.HasValue)
                    {
                        Document.Lines.UnitPrice = line.Price.Value;
                        Document.Lines.Price = line.Price.Value;
                    }

                    if (string.IsNullOrEmpty(line.TaxCode) == false)
                    {
                        Document.Lines.TaxCode = line.TaxCode;
                    }

                    if (string.IsNullOrEmpty(line.CurrencyCode) == false)
                    {
                        Document.Lines.Currency = line.CurrencyCode;
                    }

                    if (string.IsNullOrEmpty(line.Warehouse) == false)
                    {
                        Document.Lines.WarehouseCode = line.Warehouse;
                    }

                    if (string.IsNullOrEmpty(line.Project) == false)
                    {
                        Document.Lines.ProjectCode = line.Project;
                    }

                    if (line.Discount.HasValue)
                    {
                        Document.Lines.DiscountPercent = line.Discount.Value;
                    }

                    if (line.DeliveryDate.HasValue)
                    {
                        Document.Lines.ShipDate = line.DeliveryDate.Value;
                    }

                    if (line.EmployeeCode.HasValue)
                    {
                        Document.Lines.SalesPersonCode = line.EmployeeCode.Value;

                        if (line.ApplyCommission.HasValue)
                        {
                            SapRecordSet.DoQuery($"SELECT T0.Commission FROM OSLP T0 WHERE T0.Commission IS NOT NULL AND T0.SlpCode = {line.EmployeeCode.Value}");

                            if (SapRecordSet.RecordCount > 0)
                            {
                                Document.Lines.CommisionPercent = SapRecordSet.Fields.Item("Commission").IsNull() == SAPbobsCOM.BoYesNoEnum.tNO
                                    ? Convert.ToDouble(SapRecordSet.Fields.Item("Commission").Value)
                                    : 0;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(line.CostingCode1) == false)
                    {
                        Document.Lines.CostingCode = line.CostingCode1;
                    }

                    if (string.IsNullOrEmpty(line.CostingCode2) == false)
                    {
                        Document.Lines.CostingCode2 = line.CostingCode2;
                    }

                    if (string.IsNullOrEmpty(line.CostingCode3) == false)
                    {
                        Document.Lines.CostingCode3 = line.CostingCode3;
                    }

                    if (string.IsNullOrEmpty(line.CostingCode4) == false)
                    {
                        Document.Lines.CostingCode4 = line.CostingCode4;
                    }

                    if (string.IsNullOrEmpty(line.CostingCode5) == false)
                    {
                        Document.Lines.CostingCode5 = line.CostingCode5;
                    }

                    if (string.IsNullOrEmpty(line.Description) == false)
                    {
                        Document.Lines.ItemDescription = line.Description;
                    }

                    if (string.IsNullOrEmpty(line.Description) == false)
                    {
                        Document.Lines.ItemDescription = line.Description;
                    }

                    if (line.UserFields != null)
                    {
                        foreach (var field in line.UserFields)
                        {
                            if (field.Value == null)
                            {
                                Document.Lines.UserFields.Fields.Item(field.Name).SetNullValue();
                                continue;
                            }
                            Document.Lines.UserFields.Fields.Item(field.Name).Value = field.Value;
                        }
                    }

                    SapRecordSet.DoQuery(Company.IsHana
                        ? string.Format(@"SELECT * FROM OITM T0 WHERE T0.""ItemCode"" = '{0}'", line.ItemCode)
                        : string.Format(@"SELECT * FROM OITM T0 WHERE T0.ItemCode = '{0}'", line.ItemCode));

                    string ManageBatchs = SapRecordSet.Fields.Item("ManBtchNum").Value as string;
                    string ManageSeries = SapRecordSet.Fields.Item("ManSerNum").Value as string;

                    if (ManageBatchs == "Y"
                        && Document.DocObjectCode != SAPbobsCOM.BoObjectTypes.oOrders
                        && Document.DocObjectCode != SAPbobsCOM.BoObjectTypes.oStockTransfer
                        && Document.DocObjectCode != SAPbobsCOM.BoObjectTypes.oPurchaseOrders)
                    {
                        if (line.Batchs == null || line.Batchs?.Count == 0)
                        {
                            throw new Exception($"El articulo de la linea {line.ItemCode} es manejado por lotes, pero no se ha asignado ningun lote");
                        }

                        foreach (Batch batch in line.Batchs)
                        {
                            Document.Lines.BatchNumbers.BatchNumber = batch.Number;
                            Document.Lines.BatchNumbers.Quantity = batch.Quantity;

                            if (batch.ExpirationDate.HasValue)
                            {
                                Document.Lines.BatchNumbers.ExpiryDate = batch.ExpirationDate.Value;
                            }

                            Document.Lines.BatchNumbers.Add();
                        }
                    }

                    if (ManageSeries == "Y"
                        && Document.DocObjectCode != SAPbobsCOM.BoObjectTypes.oOrders
                        && Document.DocObjectCode != SAPbobsCOM.BoObjectTypes.oStockTransfer
                        && Document.DocObjectCode != SAPbobsCOM.BoObjectTypes.oPurchaseOrders)
                    {
                        if (line.Series == null || line.Series?.Count == 0)
                        {
                            throw new Exception($"El articulo de la linea {line.ItemCode} es manejado por series, pero no se ha asignado ninguna serie");
                        }

                        foreach (Serial serie in line.Series)
                        {
                            SapRecordSet.DoQuery(Company.IsHana
                                ? $"SELECT T0.\"SysNumber\", T0.\"MnfSerial\", T0.\"LotNumber\" FROM OSRN WHERE T0.\"DistNumber\" = '{serie.Number}'"
                                : $"SELECT T0.SysNumber, T0.MnfSerial, T0.LotNumber FROM OSRN T0 WHERE T0.DistNumber = '{serie.Number}'");

                            if (SapRecordSet.RecordCount == 0)
                            {
                                throw new Exception($"La serie con numero {serie.Number} del articulo {line.ItemCode} no existe");
                            }

                            Document.Lines.SerialNumbers.InternalSerialNumber = serie.Number;
                            Document.Lines.SerialNumbers.SystemSerialNumber = Convert.ToInt32(SapRecordSet.Fields.Item("SysNumber").Value);
                            Document.Lines.SerialNumbers.ManufacturerSerialNumber = SapRecordSet.Fields.Item("MnfSerial").Value as string;
                            Document.Lines.SerialNumbers.Quantity = serie.Quantity;

                            if (serie.ExpirationDate.HasValue)
                            {
                                Document.Lines.SerialNumbers.ExpiryDate = serie.ExpirationDate.Value;
                            }

                            Document.Lines.SerialNumbers.Add();
                        }
                    }

                    if (!WithPreviousLines)
                    {
                        Document.Lines.Add();
                    }
                }
            }

            private void AddDocumentLines(List<TransferRequestLine> Lines, ref SAPbobsCOM.StockTransfer Document, ref SAPbobsCOM.Recordset SapRecordSet)
            {
                bool WithPreviousLines = Document.Lines.Count > 1;

                foreach (TransferRequestLine line in Lines)
                {
                    if (WithPreviousLines)
                    {
                        Document.Lines.Add();
                    }

                    Document.Lines.ItemCode = line.ItemCode;
                    Document.Lines.Quantity = line.Quantity;
                    Document.Lines.FromWarehouseCode = line.FromWarehouseCode;
                    Document.Lines.WarehouseCode = line.ToWarehouseCode;

                    if (string.IsNullOrEmpty(line.CostingCode1) == false)
                    {
                        Document.Lines.DistributionRule = line.CostingCode1;
                    }

                    if (string.IsNullOrEmpty(line.CostingCode2) == false)
                    {
                        Document.Lines.DistributionRule2 = line.CostingCode2;
                    }

                    if (string.IsNullOrEmpty(line.CostingCode3) == false)
                    {
                        Document.Lines.DistributionRule3 = line.CostingCode3;
                    }

                    if (string.IsNullOrEmpty(line.CostingCode4) == false)
                    {
                        Document.Lines.DistributionRule4 = line.CostingCode4;
                    }

                    if (string.IsNullOrEmpty(line.CostingCode5) == false)
                    {
                        Document.Lines.DistributionRule5 = line.CostingCode5;
                    }

                    if (string.IsNullOrEmpty(line.Project) == false)
                    {
                        Document.Lines.ProjectCode = line.Project;
                    }

                    if (line.UserFields != null)
                    {
                        foreach (var field in line.UserFields)
                        {
                            if (field.Value == null)
                            {
                                Document.Lines.UserFields.Fields.Item(field.Name).SetNullValue();
                                continue;
                            }
                            Document.Lines.UserFields.Fields.Item(field.Name).Value = field.Value;
                        }
                    }

                    if (!WithPreviousLines)
                    {
                        Document.Lines.Add();
                    }
                }
            }

            public int AddAttachment(string FileLocation)
            {
                SAPbobsCOM.Attachments2 attachment = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2) as SAPbobsCOM.Attachments2;
                attachment.Lines.Add();

                attachment.Lines.FileName = Path.GetFileNameWithoutExtension(FileLocation);
                attachment.Lines.FileExtension = Path.GetExtension(FileLocation).Substring(1);
                attachment.Lines.SourcePath = Path.GetDirectoryName(FileLocation);
                attachment.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;

                if (attachment.Add() != 0)
                {
                    throw new Exception(
                        $"({SapCompany.GetLastErrorCode()}) Error agregando anexos: {SapCompany.GetLastErrorDescription()}");
                }

                return Convert.ToInt32(SapCompany.GetNewObjectKey());
            }

            public void CreateProject(Project Project)
            {
                var CompanyServices = SapCompany.GetCompanyService();
                var ProjectServices = CompanyServices.GetBusinessService(SAPbobsCOM.ServiceTypes.ProjectsService) as SAPbobsCOM.IProjectsService;
                var SapProject = ProjectServices.GetDataInterface(SAPbobsCOM.ProjectsServiceDataInterfaces.psProject) as SAPbobsCOM.Project;

                SapProject.Code = Project.Code;
                SapProject.Name = Project.Description;
                SapProject.Active = Project.Active ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;

                if (Project.ValidFrom.HasValue)
                {
                    if (Project.ValidTo.HasValue == false)
                    {
                        throw new Exception("El projecto tiene asignada la fecha de validez inicial pero no la de termino");
                    }

                    SapProject.ValidFrom = Project.ValidFrom.Value;
                    SapProject.ValidTo = Project.ValidTo.Value;
                }

                if (Project.UserFields != null && Project.UserFields?.Count > 0)
                {
                    foreach (UserField Field in Project.UserFields)
                    {
                        if (Field.Value == null)
                        {
                            SapProject.UserFields.Item(Field.Name).SetNullValue();
                            continue;
                        }
                        SapProject.UserFields.Item(Field.Name).Value = Field;
                    }
                }

                ProjectServices.AddProject(SapProject);
            }

            public void CreateBusinessPartner(BusinessPartner BusinessPartner)
            {
                try
                {
                    SAPbobsCOM.BusinessPartners Partner = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners) as SAPbobsCOM.BusinessPartners;
                    if (Partner.GetByKey(BusinessPartner.CardCode))
                    {
                        throw new Exception($"(500) Ya existe un socio de negocios con el codigo {BusinessPartner.CardCode}");
                    }

                    Partner.CardCode = BusinessPartner.CardCode;

                    if (string.IsNullOrEmpty(BusinessPartner.CardName) == false)
                    {
                        Partner.CardName = BusinessPartner.CardName;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.Comments) == false)
                    {
                        Partner.FreeText = BusinessPartner.Comments;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.CurrencyIsoCode) == false)
                    {
                        Partner.Currency = BusinessPartner.CurrencyIsoCode;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.Phone) == false)
                    {
                        Partner.Phone1 = BusinessPartner.Phone;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.OtherPhone) == false)
                    {
                        Partner.Phone2 = BusinessPartner.OtherPhone;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.Mobile) == false)
                    {
                        Partner.Cellular = BusinessPartner.Mobile;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.Email) == false)
                    {
                        Partner.EmailAddress = BusinessPartner.Email;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.Website) == false)
                    {
                        Partner.Website = BusinessPartner.Website;
                    }
                    if (BusinessPartner.IsActive.HasValue)
                    {
                        Partner.Frozen = BusinessPartner.IsActive.Value
                            ? SAPbobsCOM.BoYesNoEnum.tYES
                            : SAPbobsCOM.BoYesNoEnum.tNO;
                    }
                    if (BusinessPartner.GroupCode.HasValue)
                    {
                        Partner.GroupCode = BusinessPartner.GroupCode.Value;
                    }
                    if (BusinessPartner.CreditLine.HasValue)
                    {
                        Partner.CreditLimit = BusinessPartner.CreditLine.Value;
                    }
                    if (BusinessPartner.PriceListNum.HasValue)
                    {
                        Partner.PriceListNum = BusinessPartner.PriceListNum.Value;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.CardFName) == false)
                    {
                        Partner.CardForeignName = BusinessPartner.CardFName;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.LicTradNum) == false)
                    {
                        Partner.FederalTaxID = BusinessPartner.LicTradNum;
                    }
                    if (BusinessPartner.PaymentCondition.HasValue)
                    {
                        Partner.PayTermsGrpCode = BusinessPartner.PaymentCondition.Value;
                    }
                    if (string.IsNullOrEmpty(BusinessPartner.ProjectCode) == false)
                    {
                        Partner.ProjectCode = BusinessPartner.ProjectCode;
                    }

                    if (BusinessPartner.UserFields != null && BusinessPartner.UserFields?.Count > 0)
                    {
                        foreach (UserField field in BusinessPartner.UserFields)
                        {
                            if (field.Value == null)
                            {
                                continue;
                            }
                            Partner.UserFields.Fields.Item(field.Name).Value = field.Value;
                        }
                    }

                    if (Partner.Add() != 0)
                    {
                        throw new Exception($"({SapCompany.GetLastErrorCode()}) No es posible agregar socio de negocio. Error: {SapCompany.GetLastErrorDescription()}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error agregando socio de negocio: {ex.Message}");
                }
            }

            public Tuple<int, string> AddContactToBusinessPartner(string CardCode, Contact Contact)
            {
                try
                {
                    SAPbobsCOM.BusinessPartners Partner = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners) as SAPbobsCOM.BusinessPartners;
                    if (Partner.GetByKey(CardCode) == false)
                    {
                        throw new Exception($"(500) No existe un socio de negocios con el codigo {CardCode}");
                    }

                    SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                    SapRecordSet.DoQuery($"SELECT T0.CntctCode, T0.Name FROM OCPR T0 WHERE T0.Name = '{Contact.Name}'");

                    if (SapRecordSet.RecordCount > 0)
                    {
                        return new Tuple<int, string>(
                            Convert.ToInt32(SapRecordSet.Fields.Item("CntctCode").Value),
                            SapRecordSet.Fields.Item("Name").Value.ToString());
                    }

                    bool WithPreviousLines = Partner.ContactEmployees.Count > 1;

                    if (WithPreviousLines)
                    {
                        Partner.ContactEmployees.Add();
                    }

                    Partner.ContactEmployees.Name = Contact.Name;
                    
                    if (string.IsNullOrEmpty(Contact.FirstName) == false)
                    {
                        Partner.ContactEmployees.FirstName = Contact.FirstName;
                    }
                    if (string.IsNullOrEmpty(Contact.Title) == false)
                    {
                        Partner.ContactEmployees.Title = Contact.Title;
                    }
                    if (string.IsNullOrEmpty(Contact.MiddleName) == false)
                    {
                        Partner.ContactEmployees.MiddleName = Contact.MiddleName;
                    }
                    if (string.IsNullOrEmpty(Contact.Position) == false)
                    {
                        Partner.ContactEmployees.Position = Contact.Position;
                    }
                    if (string.IsNullOrEmpty(Contact.LastName) == false)
                    {
                        Partner.ContactEmployees.LastName = Contact.LastName;
                    }
                    if (string.IsNullOrEmpty(Contact.Address) == false)
                    {
                        Partner.ContactEmployees.Address = Contact.Address;
                    }
                    if (string.IsNullOrEmpty(Contact.Email) == false)
                    {
                        Partner.ContactEmployees.E_Mail = Contact.Email;
                    }
                    if (string.IsNullOrEmpty(Contact.Phone1) == false)
                    {
                        Partner.ContactEmployees.Phone1 = Contact.Phone1;
                    }
                    if (string.IsNullOrEmpty(Contact.Phone2) == false)
                    {
                        Partner.ContactEmployees.Phone2 = Contact.Phone2;
                    }
                    if (string.IsNullOrEmpty(Contact.Fax) == false)
                    {
                        Partner.ContactEmployees.Fax = Contact.Fax;
                    }
                    if (string.IsNullOrEmpty(Contact.MobilePhone) == false)
                    {
                        Partner.ContactEmployees.MobilePhone = Contact.MobilePhone;
                    }
                    if (Contact.DateOfBirth.HasValue)
                    {
                        Partner.ContactEmployees.DateOfBirth = Contact.DateOfBirth.Value;
                    }
                    if (string.IsNullOrEmpty(Contact.CityOfBirth) == false)
                    {
                        Partner.ContactEmployees.CityOfBirth = Contact.CityOfBirth;
                    }
                    if (string.IsNullOrEmpty(Contact.PlaceOfBirth) == false)
                    {
                        Partner.ContactEmployees.PlaceOfBirth = Contact.PlaceOfBirth;
                    }
                    if (string.IsNullOrEmpty(Contact.Profession) == false)
                    {
                        Partner.ContactEmployees.Profession = Contact.Profession;
                    }
                    if (Contact.Gender.HasValue)
                    {
                        Partner.ContactEmployees.Gender = Contact.Gender.Value == Gender.Male
                            ? SAPbobsCOM.BoGenderTypes.gt_Male
                            : SAPbobsCOM.BoGenderTypes.gt_Female;
                    }

                    Partner.ContactEmployees.Active = Contact.Active
                        ? SAPbobsCOM.BoYesNoEnum.tYES
                        : SAPbobsCOM.BoYesNoEnum.tNO;

                    if (!WithPreviousLines)
                    {
                        Partner.ContactEmployees.Add();
                    }

                    if (Partner.Update() != 0)
                    {
                        throw new Exception($"({SapCompany.GetLastErrorCode()}) No es posible agregar el contacto al socio de negocio. Error: {SapCompany.GetLastErrorDescription()}");
                    }

                    SapRecordSet.DoQuery($"SELECT T0.CntctCode, T0.Name FROM OCPR T0 WHERE T0.Name = '{Contact.Name}'");

                    return new Tuple<int, string>(
                        Convert.ToInt32(SapRecordSet.Fields.Item("CntctCode").Value),
                        SapRecordSet.Fields.Item("Name").Value.ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error agregando contacto a socio de negocio: {ex.Message}");
                }
            }

            public string AddAddressToBusinessPartner(string CardCode, Address Address)
            {
                try
                {
                    SAPbobsCOM.BusinessPartners Partner = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners) as SAPbobsCOM.BusinessPartners;
                    if (Partner.GetByKey(CardCode) == false)
                    {
                        throw new Exception($"(500) No existe un socio de negocios con el codigo {CardCode}");
                    }

                    SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                    SapRecordSet.DoQuery($"SELECT T0.Address, T0.AdresType FROM CRD1 T0 WHERE T0.Address = '{Address.AddressCode}' AND T0.CardCode = '{CardCode}' AND T0.AdresType = '{(Address.Type == AddressType.Pay ? "B" : "S")}'");

                    if (SapRecordSet.RecordCount > 0)
                    {
                        return SapRecordSet.Fields.Item("Address").Value.ToString();
                    }

                    if (string.IsNullOrEmpty(Partner.Addresses.AddressName) == false)
                    {
                        Partner.Addresses.Add();
                    }

                    Partner.Addresses.AddressName = Address.AddressCode;
                    Partner.Addresses.AddressType = Address.Type == AddressType.Ship ? SAPbobsCOM.BoAddressType.bo_ShipTo : SAPbobsCOM.BoAddressType.bo_BillTo;

                    if (string.IsNullOrEmpty(Address.Name2) == false)
                    {
                        Partner.Addresses.AddressName2 = Address.Name2;
                    }
                    if (string.IsNullOrEmpty(Address.Name3) == false)
                    {
                        Partner.Addresses.AddressName3 = Address.Name3;
                    }
                    if (string.IsNullOrEmpty(Address.Street) == false)
                    {
                        Partner.Addresses.Street = Address.Street;
                    }
                    if (string.IsNullOrEmpty(Address.Block) == false)
                    {
                        Partner.Addresses.Block = Address.Block;
                    }
                    if (string.IsNullOrEmpty(Address.Building) == false)
                    {
                        Partner.Addresses.BuildingFloorRoom = Address.Building;
                    }
                    if (string.IsNullOrEmpty(Address.City) == false)
                    {
                        Partner.Addresses.City = Address.City;
                    }
                    if (string.IsNullOrEmpty(Address.CountryCode) == false)
                    {
                        Partner.Addresses.Country = Address.CountryCode;
                    }
                    if (string.IsNullOrEmpty(Address.County) == false)
                    {
                        Partner.Addresses.County = Address.County;
                    }
                    if (string.IsNullOrEmpty(Address.StateCode) == false)
                    {
                        Partner.Addresses.State = Address.StateCode;
                    }
                    if (string.IsNullOrEmpty(Address.StreetNumber) == false)
                    {
                        Partner.Addresses.StreetNo = Address.StreetNumber;
                    }
                    if (string.IsNullOrEmpty(Address.Zip) == false)
                    {
                        Partner.Addresses.ZipCode = Address.Zip;
                    }
                    if (string.IsNullOrEmpty(Address.TaxCode) == false)
                    {
                        Partner.Addresses.TaxCode = Address.TaxCode;
                    }
                    if (string.IsNullOrEmpty(Address.NumGlobalLocation) == false)
                    {
                        Partner.Addresses.GlobalLocationNumber = Address.NumGlobalLocation;
                    }

                    if (Partner.Update() != 0)
                    {
                        throw new Exception($"({SapCompany.GetLastErrorCode()}) No es posible agregar la direccion al socio de negocio. Error: {SapCompany.GetLastErrorDescription()}");
                    }

                    SapRecordSet.DoQuery($"SELECT T0.Address FROM CRD1 T0 WHERE T0.Address = '{Address.AddressCode}' AND T0.CardCode = '{CardCode}' AND T0.AdresType = '{(Address.Type == AddressType.Pay ? "B" : "S")}'");

                    return SapRecordSet.Fields.Item("Address").Value.ToString();
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error agregando direccion a socio de negocio: {ex.Message}");
                }
            }

            public void UpdateEquipmentCard(int InsID, EquipmentCard Card)
            {
                try
                {
                    SAPbobsCOM.CustomerEquipmentCards CustomerCard = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCustomerEquipmentCards) as SAPbobsCOM.CustomerEquipmentCards;
                    SAPbobsCOM.Recordset SapRecordSet = SapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                    if (CustomerCard.GetByKey(InsID) == false)
                    {
                        throw new Exception($"(404) No existe una tarjeta de equipo con codigo {InsID}");
                    }

                    if (string.IsNullOrEmpty(Card.CustomerCode) == false)
                    {
                        CustomerCard.CustomerCode = Card.CustomerCode;
                    }
                    if (string.IsNullOrEmpty(Card.SerialNumber) == false)
                    {
                        CustomerCard.InternalSerialNum = Card.SerialNumber;
                    }
                    if (string.IsNullOrEmpty(Card.SerialNumberManufacturer) == false)
                    {
                        CustomerCard.ManufacturerSerialNum = Card.SerialNumberManufacturer;
                    }

                    if (Card.CardType.HasValue)
                    {
                        CustomerCard.ServiceBPType = Card.CardType == EquipmentCardType.Sales
                            ? SAPbobsCOM.BoEquipmentBPType.et_Sales
                            : SAPbobsCOM.BoEquipmentBPType.et_Purchasing;
                    }

                    if (Card.ContactCard.HasValue)
                    {
                        CustomerCard.ContactEmployeeCode = Card.ContactCard.Value;
                    }
                    if (Card.DefaultTechnical.HasValue)
                    {
                        CustomerCard.DefaultTechnician = Card.DefaultTechnical.Value;
                    }
                    if (Card.DefaultTerritory.HasValue)
                    {
                        CustomerCard.Defaultterritory = Card.DefaultTerritory.Value;
                    }

                    if (Card.CardStatus.HasValue) {
                        if (Card.CardStatus.Value == EquipmentCardStatus.Active)
                        {
                            CustomerCard.StatusOfSerialNumber = SAPbobsCOM.BoSerialNumberStatus.sns_Active;
                        }
                        else if (Card.CardStatus.Value == EquipmentCardStatus.InLab)
                        {
                            CustomerCard.StatusOfSerialNumber = SAPbobsCOM.BoSerialNumberStatus.sns_InLab;
                        }
                        else if (Card.CardStatus.Value == EquipmentCardStatus.Loaned)
                        {
                            CustomerCard.StatusOfSerialNumber = SAPbobsCOM.BoSerialNumberStatus.sns_Loaned;
                        }
                        else if (Card.CardStatus.Value == EquipmentCardStatus.Terminated)
                        {
                            CustomerCard.StatusOfSerialNumber = SAPbobsCOM.BoSerialNumberStatus.sns_Terminated;
                        }
                        else if (Card.CardStatus.Value == EquipmentCardStatus.Returned)
                        {
                            CustomerCard.StatusOfSerialNumber = SAPbobsCOM.BoSerialNumberStatus.sns_Returned;
                        }
                    }

                    if (Card.Address != null)
                    {
                        if (string.IsNullOrEmpty(Card.Address.City) == false)
                        {
                            CustomerCard.City = Card.Address.City;
                        }
                        if (string.IsNullOrEmpty(Card.Address.County) == false)
                        {
                            CustomerCard.County = Card.Address.County;
                        }
                        if (string.IsNullOrEmpty(Card.Address.CountryCode) == false)
                        {
                            CustomerCard.CountryCode = Card.Address.CountryCode;
                        }
                        if (string.IsNullOrEmpty(Card.Address.StateCode) == false)
                        {
                            CustomerCard.StateCode = Card.Address.StateCode;
                        }
                        if (string.IsNullOrEmpty(Card.Address.Building) == false)
                        {
                            CustomerCard.BuildingFloorRoom = Card.Address.Building;
                        }
                        if (string.IsNullOrEmpty(Card.Address.Block) == false)
                        {
                            CustomerCard.Block = Card.Address.Block;
                        }
                        if (string.IsNullOrEmpty(Card.Address.Zip) == false)
                        {
                            CustomerCard.ZipCode = Card.Address.Zip;
                        }
                        if (string.IsNullOrEmpty(Card.Address.Street) == false)
                        {
                            CustomerCard.Street = Card.Address.Street;
                        }
                    }

                    if (CustomerCard.Update() != 0)
                    {
                        throw new Exception(
                            $"({SapCompany.GetLastErrorCode()}) Error actualizando tarjeta de equipo: {SapCompany.GetLastErrorDescription()}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("("))
                    {
                        throw ex;
                    }

                    throw new Exception($"Error actualizando tarjeta de equipo: {ex.Message}");
                }
            }

            private LineReference GetLineReference(string ItemCode, DocumentReference Reference, ref SAPbobsCOM.Recordset SapRecordSet)
            {
                if (Reference.Type == ObjectType.SaleOrder)
                {
                    SapRecordSet.DoQuery(Company.IsHana
                        ? string.Format(@"SELECT T1.""DocStatus"", T0.""LineNum"" FROM RDR1 T0
                        INNER JOIN ORDR T1 ON T1.""DocEntry"" = T0.""DocEntry"" WHERE T0.""DocEntry"" = {0}
                        AND T0.""ItemCode"" = {1}", Reference.Number, ItemCode)
                        : string.Format(@"SELECT T1.DocStatus, T0.LineNum FROM RDR1 T0
                        INNER JOIN ORDR T1 ON T1.DocEntry = T0.DocEntry WHERE T0.DocEntry = {0}
                        AND T0.ItemCode = {1}", Reference.Number, ItemCode));

                    if (SapRecordSet.RecordCount == 0)
                    {
                        throw new Exception($"El documento base de la referencia para la linea del articulo {ItemCode} no existe");
                    }

                    string DocStatus = SapRecordSet.Fields.Item("DocStatus").Value as string;
                    int LineNum = Convert.ToInt32(SapRecordSet.Fields.Item("LineNum").Value);

                    return new LineReference
                    {
                        Status = DocStatus == "O" ? DocumentStatus.Open : DocumentStatus.Closed,
                        LineNum = LineNum
                    };
                }

                return null;
            }
        }
    }

    public class OperationResult
    {
        public object Result { get; set; }
        public Exception Exception { get; set; }
        public HookResult PreExecutionResult { get; set; }
        public HookResult PostExecutionResult { get; set; }
    }

    public class HookResult
    {
        public Exception Exception { get; set; }
        public object Result { get; set; }
    }
}
