using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using HitchAtmApi.Models;
using HitchAtmApi.ViewModel;

namespace HitchAtmApi.SwaggerExamples.SalesOrders.Requests
{
    public class GetCustomerRequestExample : IExamplesProvider<CustomerInput>
    {
        public CustomerInput GetExamples()
        {
            return new CustomerInput
            {
                LicTradNum = "11111111-1",
            };
        }
    }

    public class GetProductRequestExample : IExamplesProvider<ProductInput>
    {
        public ProductInput GetExamples()
        {
            return new ProductInput
            {
                ItemCode = "NI40370-M094"
            };
        }
    }

    public class UpdateEquipmentCardRequestExample : IExamplesProvider<CardInput>
    {
        public CardInput GetExamples()
        {
            return new CardInput
            {
                insID = 5187,
                Status = "L",
                Address = new Address
                {
                    Street = "Avda. Vitacura 5951",
                    City = "Santiago",
                    CountryCode = "CL",
                    StateCode = "13"
                }
            };
        }
    }

    public class CreateSaleOrderRequestExample : IExamplesProvider<SaleOrder>
    {
        public SaleOrder GetExamples()
        {
            return new SaleOrder
            {
                CardCode = "76493007",
                CNTCCode = 12977,
                DocDate = DateTime.Now,
                DocDueDate = DateTime.Now.AddDays(30),
                TaxDate = DateTime.Now,
                Vendedor = 6,
                ShipToCode = "",
                PayToCode = "",
                PartSuply = false,
                CodSF = "TESTHITCH01",
                DescuentoTotal = "10%",
                DescuentoCabecera = 10.0,
                CancFaltaStock = false,
                CapacitacionReq = "01",
                InstalacionReq = "01",
                ContactSN = "Marcos M",
                DateOCCLiente = DateTime.Now,
                DocOCCliente = "JVBERi0xLjMNCiXi48/TDQoNCjEgMCBvYmoNCjw8DQovVHlwZ...",
                DateRecepcionOC = DateTime.Now,
                DirecEntregaFactura = "Direccion 1234 Santiago",
                ExistenMultas = "01",
                GarantiaPactada = "1",
                LeasingATM = false,
                MantPreventivo = "01",
                NomSN = "Marcos M",
                NumCotizacionProv = "5582",
                NumVisitasAnoGarantia = 2,
                RutSN = "11111111-1",
                OCcliente = "100101",
                Project = "ZM",
                Detail = new List<SaleOrderDetail>
                {
                    new SaleOrderDetail
                    {
                        ItemCode = "NI40370-M094",
                        Description = "Nidek Pliable Cup (Red)",
                        Almacen = "001",
                        DateEntrega = DateTime.Now,
                        Descuento = "10%",
                        UnitPrice = 10000,
                        Quantity = 12
                    }
                }
            };
        }
    }

    public class CreatePurchaseOrderRequestExample : IExamplesProvider<PurchaseOrder>
    {
        public PurchaseOrder GetExamples()
        {
            return new PurchaseOrder
            {
                CardCode = "9999111P",
                DocDate = DateTime.Now,
                DocDueDate = DateTime.Now.AddDays(30),
                TaxDate = DateTime.Now,
                CodSF = "TESTHITCH02",
                CiudadesDest = "Santiago",
                Discount = "10%",
                FechaLlegada = DateTime.Now.AddDays(20),
                FechaSalida = DateTime.Now,
                HoraSalidaStgo = "1200",
                HoraUltimaVisita = "1000",
                NumVisitas = 12,
                ObsCobertura = "Observaciones de prueba",
                OCOrigen = 12345,
                OportVentas = "12345",
                Serie = 44,
                TipoSolic = "02",
                Detail = new List<PurchaseOrderDetail>
                {
                    new PurchaseOrderDetail
                    {
                        ItemCode = "NI40370-M094",
                        Quantity = 12,
                        Dim1 = "AGR",
                        Dim2 = "MPU",
                        Dim3 = "UR"
                    }
                }
            };
        }
    }

    public class CreateTransferRequestRequestExample : IExamplesProvider<TransferRequest>
    {
        public TransferRequest GetExamples()
        {
            return new TransferRequest
            {
                CardCode = "9999111P",
                DocDate = DateTime.Now,
                TaxDate = DateTime.Now,
                DocDueDate = DateTime.Now,
                CodSF = "TESTHITCH03",
                AlmacenDestino = "001",
                AlmacenOrigen = "002",
                Anexo = "JVBERi0xLjMNCiXi48/TDQoNCjEgMCBvYmoNCjw8DQovVHlwZ...",
                CodeSN = "3318907",
                Contacto = 12,
                DateStart = DateTime.Now,
                DateEnd = DateTime.Now,
                ImlMeno = "Comentarios",
                LlamadaServ = 12,
                NumOVSF = 12345,
                Observaciones = "Observaciones",
                RazonSocial = "Razon social",
                SlpCode = 12,
                TipoPD = "02",
                Ubicacion = "Prueba",
                Detail = new List<TransferRequestDetail>
                {
                    new TransferRequestDetail
                    {
                        ItemCode = "NI40370-M094",
                        Quantity = 12,
                        Almacen = "001",
                        AlmacenDest = "002"
                    }
                }
            };
        }
    }
}
