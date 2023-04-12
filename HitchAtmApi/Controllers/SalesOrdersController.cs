using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using HitchAtmApi.Lib;
using HitchAtmApi.Models;
using HitchAtmApi.SwaggerExamples.Responses;

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
        async public Task<IActionResult> SaveSaleOrder([FromBody] SaleOrder Order)
        {
            try
            {
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

                /*SaleOrder orderInDb = await SalesOrdersService.GetSaleOrder(Order.CodSF);
                Notification notification = null;

                if (orderInDb != null)
                {
                    notification = await NotificationsService.GetNotification(orderInDb.Id.Value, "Orden de venta");
                    orderInDb.Detail = await SalesOrdersService.GetSaleOrderDetail(orderInDb.Id.Value);

                    if (notification != null)
                    {
                        if (notification.DocNum.HasValue)
                        {
                            try
                            {
                                SapService.UpdateSaleOrder(notification.DocEntry.Value, Order);
                                notification.Status = "Finalizada";
                                notification.Stage = "Registro actualizado";
                                notification.FinishTime = DateTime.Now;

                                await NotificationsService.UpdateNotification(notification);

                                await NotificationsService.Log(new NotificationLog
                                {
                                    CreateTime = DateTime.Now,
                                    Message = null,
                                    NotificationId = notification.Id,
                                    Operation = "Actualizando registro",
                                    Status = "OK",
                                });
                            }
                            catch (Exception ex)
                            {
                                await NotificationsService.Log(new NotificationLog
                                {
                                    CreateTime = DateTime.Now,
                                    Message = $"{ex.Message}, {ex.StackTrace}",
                                    NotificationId = notification.Id,
                                    Operation = "Actualizando orden de venta en SAP",
                                    Status = "ERROR",
                                });

                                notification.Status = "Fallida";
                                await NotificationsService.UpdateNotification(notification);

                                return new HttpResponse
                                    .Error(500, "No logro actualizarse la orden de venta en SAP", ex.Message)
                                    .Send();
                            }

                            return new HttpResponse.Response<int>
                            {
                                Data = notification.DocNum.Value,
                                Error = null,
                                Status = 200,
                                Message = "Registro actualizado"
                            }.Send();
                        }
                        else
                        {
                            await SalesOrdersService.DeleteOrder(orderInDb.Id.Value);

                            string Doc = Order.DocOCCliente;
                            Order.DocOCCliente = null;
                            Order.Id = await SalesOrdersService.SaveSaleOrder(Order);
                            Order.DocOCCliente = Doc;

                            foreach (var line in Order.Detail)
                            {
                                line.OrderId = Order.Id.Value;
                                line.Id = await SalesOrdersService.SaveSaleOrderLine(line);
                            }

                            orderInDb = Order;

                            notification.RefId = Order.Id.Value;
                            await NotificationsService.UpdateNotification(notification);
                        }
                    }
                }

                long orderId = 0;

                if (orderInDb == null)
                {
                    string Doc = Order.DocOCCliente;
                    Order.DocOCCliente = null;
                    Order.Id = await SalesOrdersService.SaveSaleOrder(Order);
                    Order.DocOCCliente = Doc;
                    
                    foreach (var line in Order.Detail)
                    {
                        line.OrderId = Order.Id;
                        line.Id = await SalesOrdersService.SaveSaleOrderLine(line);
                    }

                    orderInDb = Order;
                    orderId = orderInDb.Id.Value;
                }
                else
                {
                    orderId = orderInDb.Id.Value;
                }

                if (notification == null)
                {
                    await NotificationsService.AddNotification(new Notification
                    {
                        CreateTime = DateTime.Now,
                        Stage = "Registrando datos orden de venta",
                        Status = "Pendiente",
                        Steps = 1,
                        RefType = "Orden de venta",
                        RefId = orderId,
                        FinishTime = null,
                        DocEntry = null,
                        DocNum = null
                    });

                    notification = await NotificationsService.GetNotification(orderInDb.Id.Value, "Orden de venta");
                }
                else
                {
                    notification = await NotificationsService.GetNotification(orderInDb.Id.Value, "Orden de venta");
                    notification.Steps += 1;
                    await NotificationsService.UpdateNotification(notification);
                }*/

                Tuple<int, int> Values = null;

                try
                {
                    Values = SapService.CreateSaleOrder(Order);
                    /*notification.DocEntry = Values.Item1;
                    notification.DocNum = Values.Item2;
                    notification.Status = "Finalizada";
                    notification.Stage = "Registro finalizado";
                    notification.FinishTime = DateTime.Now;

                    await NotificationsService.UpdateNotification(notification);

                    await NotificationsService.Log(new NotificationLog
                    {
                        CreateTime = DateTime.Now,
                        Message = null,
                        NotificationId = notification.Id,
                        Operation = "Finalizando registro",
                        Status = "OK",
                    });*/
                }
                catch (Exception ex)
                {
                    /*await NotificationsService.Log(new NotificationLog
                    {
                        CreateTime = DateTime.Now,
                        Message = $"{ex.Message}, {ex.StackTrace}",
                        NotificationId = notification.Id,
                        Operation = "Creando orden de venta en SAP",
                        Status = "ERROR",
                    });

                    notification.Status = "Fallida";
                    await NotificationsService.UpdateNotification(notification);*/

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
