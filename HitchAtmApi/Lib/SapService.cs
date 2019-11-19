using System;
using System.Collections.Generic;
using System.Linq;
using HitchAtmApi.Models;
using HitchSapB1Lib;
using HitchSapB1Lib.Operations;
using HitchSapB1Lib.Enums;
using HitchSapB1Lib.Objects.Marketing;
using HitchSapB1Lib.Objects.Definition;
using HitchSapB1Lib.Objects.Inventory;

namespace HitchAtmApi.Lib
{
    public class SapService
    {
        HitchSapB1Lib.ConnectionParameters DefaultConnectionParameters;

        public SapService(HitchSapB1Lib.ConnectionParameters connectionParameters)
        {
            DefaultConnectionParameters = connectionParameters;
        }

        public Tuple<int, int> CreateSaleOrder(HitchAtmApi.Models.SaleOrder Order)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                CreateSaleOrderOperation createSaleOrderOperation = new CreateSaleOrderOperation();

                HitchSapB1Lib.Objects.Marketing.SaleOrder SapOrder = new HitchSapB1Lib.Objects.Marketing.SaleOrder
                {
                    CustomerCode = Order.CardCode,
                    DocumentDate = Order.DocDate,
                    DueDate = Order.DocDueDate,
                    TaxDate = Order.TaxDate,
                    Discount = null,
                    PayToCode = Order.PayToCode,
                    ShipToCode = Order.ShipToCode,
                    IsPartialDelivery = Order.PartSuply.HasValue ? Order.PartSuply.Value : false,
                    CurrencySource = CurrencySource.Customer,
                    Comment = null,
                    OwnerCode = null,
                    CustomerReferenceNumber = null,
                    SalesEmployeeCode = Order.Vendedor,
                    ContactCode = Order.CNTCCode,
                    Serie = null,
                    UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                    Lines = Order.Detail.Select(det => {
                        var line = new DocumentLine
                        {
                            ItemCode = det.ItemCode,
                            Quantity = det.Quantity,
                            Discount = null,
                            Description = det.Description,
                            Warehouse = det.Almacen,
                            Price = Convert.ToDouble(det.UnitPrice),
                            DeliveryDate = det.DateEntrega,
                            Batchs = null,
                            CurrencyCode = null,
                            UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                            Reference = null,
                            Series = null
                        };

                        if (string.IsNullOrEmpty(det.Descuento) == false)
                        {
                            line.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                            {
                                Name = "U_Dscto",
                                Value = det.Descuento
                            });
                        }

                        return line;
                    }).ToList(),
                };

                if (string.IsNullOrEmpty(Order.CapacitacionReq) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCR",
                        Value = Order.CapacitacionReq
                    });
                }

                if (string.IsNullOrEmpty(Order.InstalacionReq) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOIR",
                        Value = Order.InstalacionReq
                    });
                }

                if (string.IsNullOrEmpty(Order.GarantiaPactada) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOGP",
                        Value = Order.GarantiaPactada
                    });
                }

                if (string.IsNullOrEmpty(Order.MantPreventivo) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOMP",
                        Value = Order.MantPreventivo
                    });
                }

                if (Order.NumVisitasAnoGarantia.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBON",
                        Value = Order.NumVisitasAnoGarantia.Value
                    });
                }

                if (string.IsNullOrEmpty(Order.OCcliente) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOC1",
                        Value = Order.OCcliente
                    });
                }

                if (string.IsNullOrEmpty(Order.DocOCCliente) == false)
                {
                    string fileLocation = Utils.SavePDFAttachment(
                        string.IsNullOrEmpty(Order.OCcliente) == false
                            ? Order.OCcliente
                            : System.Guid.NewGuid().ToString(), Order.DocOCCliente);

                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOCC",
                        Value = fileLocation
                    });
                }

                if (string.IsNullOrEmpty(Order.ExistenMultas) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOMULTA",
                        Value = Order.ExistenMultas
                    });
                }

                if (string.IsNullOrEmpty(Order.Project) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOZONPD",
                        Value = Order.Project
                    });
                }

                if (Order.DateOCCLiente.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFOCC",
                        Value = Order.DateOCCLiente.Value
                    });
                }

                if (Order.DateRecepcionOC.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFRE",
                        Value = Order.DateRecepcionOC.Value
                    });
                }

                if (string.IsNullOrEmpty(Order.ContactSN) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBO_CONT",
                        Value = Order.ContactSN
                    });
                }

                if (string.IsNullOrEmpty(Order.RutSN) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBORUT",
                        Value = Order.RutSN
                    });
                }

                if (string.IsNullOrEmpty(Order.NomSN) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBONOMSN",
                        Value = Order.NomSN
                    });
                }

                if (string.IsNullOrEmpty(Order.NumCotizacionProv) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCOTPROV",
                        Value = Order.NumCotizacionProv
                    });
                }

                if (Order.CancFaltaStock.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCFE",
                        Value = Order.CancFaltaStock.Value ? "S" : "N"
                    });
                }

                if (Order.LeasingATM.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_TIPOVTA",
                        Value = Order.LeasingATM.Value ? 1 : 0
                    });
                }

                if (string.IsNullOrEmpty(Order.DirecEntregaFactura) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBODIRENTFACT",
                        Value = Order.DirecEntregaFactura
                    });
                }

                if (string.IsNullOrEmpty(Order.Descuento) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_DsctItmT",
                        Value = Order.Descuento
                    });
                }

                createSaleOrderOperation.SaleOrder = SapOrder;
                createSaleOrderOperation.PreExecutionHook = null;
                createSaleOrderOperation.PostExecutionHook = null;
                createSaleOrderOperation.Company = company;
                createSaleOrderOperation.Start();

                return new Tuple<int, int>(createSaleOrderOperation.DocEntry.Value,
                    createSaleOrderOperation.DocNum.Value);
            }
        }

        public Tuple<int, int> CreatePurchaseOrder(HitchAtmApi.Models.PurchaseOrder Order)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                CreatePurchaseOrderOperation createPurchaseOrderOperation = new CreatePurchaseOrderOperation();

                HitchSapB1Lib.Objects.Shopping.PurchaseOrder SapOrder = new HitchSapB1Lib.Objects.Shopping.PurchaseOrder
                {
                    ProviderCode = Order.CardCode,
                    DocumentDate = Order.DocDate,
                    DueDate = Order.DocDueDate,
                    TaxDate = Order.TaxDate,
                    Discount = null,
                    PayToCode = null,
                    ShipToCode = null,
                    CurrencySource = CurrencySource.Customer,
                    Comment = null,
                    OwnerCode = null,
                    ProviderReferenceNumber = null,
                    BuyEmployeeCode = null,
                    ContactCode = null,
                    Serie = Order.Serie,
                    UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                    Lines = Order.Detail.Select(det =>
                    {
                        var line = new DocumentLine
                        {
                            ItemCode = det.ItemCode,
                            Quantity = det.Quantity,
                            Discount = null,
                            Description = null,
                            Price = null,
                            DeliveryDate = null,
                            Batchs = null,
                            CurrencyCode = null,
                            UserFields = null,
                            Reference = null,
                            Series = null,
                            CostingCode1 = det.Dim1,
                            CostingCode2 = det.Dim2,
                            CostingCode3 = det.Dim3
                        };

                        return line;
                    }).ToList(),
                };

                if (string.IsNullOrEmpty(Order.TipoSolic) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBO_TIPOSR",
                        Value = Order.TipoSolic
                    });
                }

                if (Order.FechaSalida.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFECHSAL",
                        Value = Order.FechaSalida.Value
                    });
                }

                if (Order.FechaLlegada.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFECHLLEG",
                        Value = Order.FechaLlegada.Value
                    });
                }

                if (Order.NumVisitas.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBONUMVIS",
                        Value = Order.NumVisitas.Value
                    });
                }

                if (string.IsNullOrEmpty(Order.OportVentas) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBODETON",
                        Value = Order.OportVentas
                    });
                }

                if (string.IsNullOrEmpty(Order.ObsCobertura) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOBCOB",
                        Value = Order.ObsCobertura
                    });
                }

                if (string.IsNullOrEmpty(Order.HoraSalidaStgo) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOHORSAL",
                        Value = Order.HoraSalidaStgo
                    });
                }

                if (string.IsNullOrEmpty(Order.HoraUltimaVisita) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOHORLLEG",
                        Value = Order.HoraUltimaVisita
                    });
                }

                if (string.IsNullOrEmpty(Order.CiudadesDest) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCIUDES",
                        Value = Order.CiudadesDest
                    });
                }

                if (Order.OCOrigen.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOCORIGEN",
                        Value = Order.OCOrigen.Value
                    });
                }

                if (string.IsNullOrEmpty(Order.Discount) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_DsctItmT",
                        Value = Order.Discount
                    });
                }

                createPurchaseOrderOperation.PurchaseOrder = SapOrder;
                createPurchaseOrderOperation.Company = company;
                createPurchaseOrderOperation.Start();

                return new Tuple<int, int>(createPurchaseOrderOperation.DocEntry.Value,
                    createPurchaseOrderOperation.DocNum.Value);
            }
        }

        public Tuple<int, int> CreateTransferRequest(HitchAtmApi.Models.TransferRequest Transfer)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                CreateTransferRequestOperation createTransferRequestOperation = new CreateTransferRequestOperation();

                HitchSapB1Lib.Objects.Inventory.TransferRequest SapTransferRequest = new HitchSapB1Lib.Objects.Inventory.TransferRequest
                {
                    CustomerCode = Transfer.CardCode,
                    ContactCode = Transfer.Contacto,
                    Comment = null,
                    DocumentDate = Transfer.DocDate,
                    DueDate = Transfer.DocDueDate,
                    TaxDate = Transfer.TaxDate,
                    StartDeliveryDate = null,
                    EndDeliveryDate = null,
                    FromWarehouseCode = Transfer.AlmacenOrigen,
                    ToWarehouseCode = Transfer.AlmacenDestino,
                    JournalMemo = Transfer.ImlMeno,
                    PriceListCode = null,
                    SalesEmployeeCode = Convert.ToInt32(Transfer.SlpCode),
                    Serie = null,
                    ShipToCode = null,
                    UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                    Attachments = new List<string>(),
                    Lines = Transfer.Detail.Select(det => new TransferRequestLine
                    {
                        ItemCode = det.ItemCode,
                        Quantity = det.Quantity,
                        FromWarehouseCode = det.Almacen,
                        ToWarehouseCode = det.AlmacenDest,
                        Reference = null,
                        Batchs = null,
                        Series = null,
                        UserFields = null
                    }).ToList()
                };

                if (string.IsNullOrEmpty(Transfer.TipoPD) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOTIPOPD",
                        Value = Transfer.TipoPD
                    });
                }

                if (string.IsNullOrEmpty(Transfer.TipoPD) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOTIPOPD",
                        Value = Transfer.TipoPD
                    });
                }

                if (string.IsNullOrEmpty(Transfer.CodeSN) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCODCLIPD",
                        Value = Transfer.TipoPD
                    });
                }

                if (string.IsNullOrEmpty(Transfer.RazonSocial) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBONOMCLIPD",
                        Value = Transfer.RazonSocial
                    });
                }

                if (Transfer.DateStart.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFINIPD",
                        Value = Transfer.DateStart.Value
                    });
                }

                if (Transfer.DateEnd.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFTERPD",
                        Value = Transfer.DateEnd.Value
                    });
                }

                if (string.IsNullOrEmpty(Transfer.Ubicacion) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOUBIPD",
                        Value = Transfer.Ubicacion
                    });
                }

                if (string.IsNullOrEmpty(Transfer.Observaciones) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOBSPD",
                        Value = Transfer.Observaciones
                    });
                }

                if (Transfer.NumOVSF.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOPVE",
                        Value = Transfer.NumOVSF.Value
                    });
                }

                if (Transfer.LlamadaServ.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOLLSER",
                        Value = Transfer.LlamadaServ.Value
                    });
                }

                if (string.IsNullOrEmpty(Transfer.Anexo) == false)
                {
                    string fileLocation = Utils.SavePDFAttachment(
                        Guid.NewGuid().ToString(), Transfer.Anexo);

                    SapTransferRequest.Attachments.Add(fileLocation);
                }

                createTransferRequestOperation.TransferRequest = SapTransferRequest;
                createTransferRequestOperation.Company = company;
                createTransferRequestOperation.Start();

                return new Tuple<int, int>(createTransferRequestOperation.DocEntry.Value,
                    createTransferRequestOperation.DocNum.Value);
            }
        }

        public Customer GetCustomer(string LicTradeNum)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                return company.QueryOneResult<Customer>(string.Format(
                    @"SELECT T0.CardCode, T0.CardName, T0.CardFName, T0.LicTradNum FROM OCRD T0
                    WHERE T0.LicTradNum = '{0}'", LicTradeNum));
            }
        }

        public Product GetProduct(string ItemCode)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                return company.QueryOneResult<Product>(string.Format(
                    @"SELECT T0.ItemCode, T0.ItemName, (CASE T0.frozenFor WHEN 'Y' THEN 0 ELSE 1 END) AS ""StatusProduct"" FROM OITM T0
                    WHERE T0.ItemCode = '{0}'", ItemCode));
            }
        }

        public void UpdateCard(int InsID, string Status, HitchAtmApi.Models.Address Address)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                UpdateEquipmentCardOperation updateEquipmentCardOperation = new UpdateEquipmentCardOperation();

                EquipmentCardStatus CardStatus = EquipmentCardStatus.Active;

                if (Status.Equals("A"))
                {
                    CardStatus = EquipmentCardStatus.Active;
                }
                else if (Status.Equals("I"))
                {
                    CardStatus = EquipmentCardStatus.InLab;
                }
                else if (Status.Equals("L"))
                {
                    CardStatus = EquipmentCardStatus.Loaned;
                }
                else if (Status.Equals("R"))
                {
                    CardStatus = EquipmentCardStatus.Returned;
                }
                else if (Status.Equals("T"))
                {
                    CardStatus = EquipmentCardStatus.Terminated;
                }
                else
                {
                    throw new Exception(
                        "El valor asignado al estado de la tarjeta de equipo no es valido. Valores validos (A: Activo, I: En laboratorio, L: en prestamo, R: Devuelto, T: Terminado");
                }

                HitchSapB1Lib.Objects.Services.EquipmentCard SapCardEquipment = new HitchSapB1Lib.Objects.Services.EquipmentCard
                {
                    CardStatus = CardStatus,
                    Address = Address != null
                     ? new HitchSapB1Lib.Objects.Definition.Address
                     {
                         Block = Address.Block,
                         Building = Address.Building,
                         City = Address.City,
                         Street = Address.Street,
                         StateCode = Address.StateCode,
                         CountryCode = Address.CountryCode,
                         County = Address.County,
                         StreetNumber = Address.StreetNumber,
                         Zip = Address.Zip
                     }
                     : null
                };

                updateEquipmentCardOperation.UpdateEquipmentCardParams = new UpdateEquipmentCardParams
                {
                    CardId = InsID,
                    EquipmentCard = SapCardEquipment
                };
                updateEquipmentCardOperation.Company = company;
                updateEquipmentCardOperation.Start();
            }
        }
    }
}
