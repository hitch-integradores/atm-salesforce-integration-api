using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using HitchAtmApi.Lib;
using HitchAtmApi.Models;
using HitchAtmApi.SwaggerExamples.Responses;
using System.Threading;
using Hangfire;

namespace HitchAtmApi.Controllers
{
    [Route("hitch/sales_orders")]
    [EnableCors("AllowAllOrigins")]
    [Produces("application/json")]
    [ApiController]
    public class SalesOrdersController : ControllerBase
    {
        private readonly SalesOrdersService SalesOrdersService;
        private readonly NotificationsService NotificationsService;
        private readonly SapService SapService;

        public SalesOrdersController(
            SalesOrdersService salesOrdersService,
            NotificationsService notificationsService,
            SapService sapService)
        {
            SalesOrdersService = salesOrdersService;
            NotificationsService = notificationsService;
            SapService = sapService;
        }

        /// <summary>
        /// Metodo para registrar nuevas ordenes de venta en SAP
        /// </summary>
        /// <response code="200">La orden de venta fue registrada correctamente</response>
        /// <response code="400">Los datos de entrada no son validos</response>
        /// <response code="500">Ocurrio un error y la orden de venta no puede ser registrada</response>
        /// <param name="Order">Datos de la orden de venta</param>
        [HttpPost]
        [ProducesResponseType(typeof(SaleOrderSuccessResponse), 200)]
        [ProducesResponseType(typeof(SaleOrderErrorResponse), 500)]
        [ProducesResponseType(typeof(SaleOrderBadResponse), 400)]
        public IActionResult SaveSaleOrder([FromBody] SaleOrder Order)
        {
            try
            {
                if (string.IsNullOrEmpty(Order.CardCode))
                {
                    if (string.IsNullOrEmpty(Order.RutSN))
                    {
                        return new HttpResponse
                       .Error(
                           400,
                           "Los datos de entrada no son validos",
                           "El valor del campo \"RutSN\" no puede estar vacio")
                       .Send();
                    }

                    string Rut = new string(Order.RutSN.Where(char.IsDigit).ToArray());
                    
                    if (Rut.Length > 1)
                    {
                        Rut = Rut.Substring(0, Rut.Length - 1);
                    }
                    
                    Order.CardCode = Rut;
                }
                if (string.IsNullOrEmpty(Order.CodSF))
                {
                    return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "El valor del campo \"CodSF\" no puede estar vacio")
                        .Send();
                }
                if (string.IsNullOrEmpty(Order.CardCode))
                {
                    return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "El valor del campo \"CardCode\" no puede estar vacio")
                        .Send();
                }
                if (Order.DocDate == null)
                {
                    return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "El valor del campo \"DocDate\" no puede estar vacio")
                        .Send();
                }
                if (Order.DocDueDate == null)
                {
                    return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "El valor del campo \"DocDueDate\" no puede estar vacio")
                        .Send();
                }
                if (Order.TaxDate == null)
                {
                    return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "El valor del campo \"TaxDate\" no puede estar vacio")
                        .Send();
                }
                if ((Order.Detail == null || Order.Detail?.Count == 0))
                {
                    return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "Deben indicarse uno o mas elementos en el campo \"Detail\"")
                        .Send();
                }
                if (string.IsNullOrEmpty(Order.DocumentoDespacho) == false)
                {
                    if (Order.DocumentoDespacho != "GD" &&
                        Order.DocumentoDespacho != "FV" &&
                        Order.DocumentoDespacho != "FR")
                    {
                        return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "El valor del campo \"DocumentoDespacho\" no es valido. Valores permitidos son GD, FV o FR")
                        .Send();
                    }
                }

                if (Order.Attachments?.Count > 0)
                {
                    var InvalidFiles = Order.Attachments.Any(a => string.IsNullOrEmpty(a.Content) || string.IsNullOrEmpty(a.Filename));

                    if (InvalidFiles)
                    {
                        return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "Uno de los items del campo \"Attachments\" no es valido")
                        .Send();
                    }
                }
                
                if (Order.PartSuply.HasValue == false)
                {
                    Order.PartSuply = false;
                }

                Tuple<int, int, int, string, string> Values = null;

                try
                {
                    Values = SapService.CreateSaleOrder(Order);
                    
                    BackgroundJob.Enqueue<IntegrationResultJob>((x) => x.IntegrationJob(Order.CardCode, Values.Item3, Values.Item4, Values.Item5));
                }
                catch (Exception ex)
                {
                    return new HttpResponse
                        .Error(500, "No logro registrarse la orden de venta en SAP", ex.Message)
                        .Send();
                }

                return new HttpResponse.Response<int>
                {
                    Data = Values.Item2,
                    Error = null,
                    Status = 200,
                    Message = "Numero documento orden de venta en SAP"
                }.Send();
            }
            catch (Exception err)
            {
                return new HttpResponse
                    .Error(500, "No logro registrarse la orden de venta en SAP", $"{err.Message}\n{err.StackTrace}")
                    .Send();
            }
        }
    }
}
