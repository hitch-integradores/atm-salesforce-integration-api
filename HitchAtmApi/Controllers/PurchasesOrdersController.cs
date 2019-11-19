using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using HitchAtmApi.Lib;
using HitchAtmApi.Models;
using HitchAtmApi.SwaggerExamples.Responses;

namespace HitchAtmApi.Controllers
{
    [Route("hitch/purchases_orders")]
    [EnableCors("AllowAllOrigins")]
    [Produces("application/json")]
    [ApiController]
    public class PurchasesOrdersController : ControllerBase
    {
        private readonly PurchasesOrdersService PurchasesOrdersService;
        private readonly NotificationsService NotificationsService;
        private readonly SapService SapService;

        public PurchasesOrdersController(
            PurchasesOrdersService purchasesOrdersService,
            NotificationsService notificationsService,
            SapService sapService)
        {
            PurchasesOrdersService = purchasesOrdersService;
            NotificationsService = notificationsService;
            SapService = sapService;
        }

        /// <summary>
        /// Metodo para registrar nuevas ordenes de compra en SAP
        /// </summary>
        /// <response code="200">La orden de compra fue registrada correctamente</response>
        /// <response code="400">Los datos de entrada no son validos</response>
        /// <response code="500">Ocurrio un error y la orden de compra no puede ser registrada</response>
        /// <param name="Order">Datos de la orden de compra</param>
        [HttpPost]
        [ProducesResponseType(typeof(PurchaseOrderSuccessResponse), 200)]
        [ProducesResponseType(typeof(PurchaseOrderErrorResponse), 500)]
        [ProducesResponseType(typeof(PurchaseOrderBadResponse), 400)]
        async public Task<IActionResult> SavePurchaseOrder([FromBody] PurchaseOrder Order)
        {
            try
            {
                if (string.IsNullOrEmpty(Order.CodSF) ||
                    string.IsNullOrEmpty(Order.CardCode) ||
                    Order.DocDate == null ||
                    Order.DocDueDate == null ||
                    Order.TaxDate == null ||
                    (Order.Detail == null || Order.Detail?.Count == 0))
                {
                    return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "Uno de los campos requeridos no tienen un valor asignado")
                        .Send();
                }

                PurchaseOrder orderInDb = await PurchasesOrdersService.GetPurchaseOrder(Order.CodSF);
                Notification notification = null;

                if (orderInDb != null)
                {
                    notification = await NotificationsService.GetNotification(orderInDb.Id.Value, "Orden de compra");
                    orderInDb.Detail = await PurchasesOrdersService.GetPurchaseOrderDetail(orderInDb.Id.Value);

                    if (notification != null)
                    {
                        if (notification.DocNum.HasValue)
                        {
                            return new HttpResponse.Response<int>
                            {
                                Data = notification.DocNum.Value,
                                Error = null,
                                Status = 200,
                                Message = "Numero documento orden de compra en SAP"
                            }.Send();
                        }
                        else
                        {
                            await PurchasesOrdersService.DeleteOrder(orderInDb.Id.Value);
                            Order.Id = await PurchasesOrdersService.SavePurchaseOrder(Order);

                            foreach (var line in Order.Detail)
                            {
                                line.OrderId = Order.Id.Value;
                                line.Id = await PurchasesOrdersService.SavePurchaseOrderLine(line);
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
                    Order.Id = await PurchasesOrdersService.SavePurchaseOrder(Order);

                    foreach (var line in Order.Detail)
                    {
                        line.OrderId = Order.Id;
                        line.Id = await PurchasesOrdersService.SavePurchaseOrderLine(line);
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
                        Stage = "Registrando datos orden de compra",
                        Status = "Pendiente",
                        Steps = 1,
                        RefType = "Orden de compra",
                        RefId = orderId,
                        FinishTime = null,
                        DocEntry = null,
                        DocNum = null
                    });

                    notification = await NotificationsService.GetNotification(orderInDb.Id.Value, "Orden de compra");
                }
                else
                {
                    notification = await NotificationsService.GetNotification(orderInDb.Id.Value, "Orden de compra");
                    notification.Steps += 1;
                    await NotificationsService.UpdateNotification(notification);
                }

                Tuple<int, int> Values = null;

                try
                {
                    Values = SapService.CreatePurchaseOrder(orderInDb);
                    notification.DocEntry = Values.Item1;
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
                    });
                }
                catch (Exception ex)
                {
                    await NotificationsService.Log(new NotificationLog
                    {
                        CreateTime = DateTime.Now,
                        Message = $"{ex.Message}, {ex.StackTrace}",
                        NotificationId = notification.Id,
                        Operation = "Creando orden de compra en SAP",
                        Status = "ERROR",
                    });

                    notification.Status = "Fallida";
                    await NotificationsService.UpdateNotification(notification);

                    return new HttpResponse
                        .Error(500, "No logro registrarse la orden de compra en SAP", ex.Message)
                        .Send();
                }

                return new HttpResponse.Response<int>
                {
                    Data = Values.Item2,
                    Error = null,
                    Status = 200,
                    Message = "Numero documento orden de compra en SAP"
                }.Send();
            }
            catch (Exception err)
            {
                return new HttpResponse
                    .Error(500, "No logro registrarse la orden de compra en SAP", $"{err.Message}\n{err.StackTrace}")
                    .Send();
            }
        }
    }
}
