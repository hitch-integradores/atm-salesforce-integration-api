using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using HitchAtmApi.Lib;
using HitchAtmApi.Models;
using HitchAtmApi.SwaggerExamples.Responses;

namespace HitchAtmApi.Controllers
{
    [Route("hitch/transfers_requests")]
    [EnableCors("AllowAllOrigins")]
    [Produces("application/json")]
    [ApiController]
    public class TransfersRequestsController : ControllerBase
    {
        private readonly TransfersRequestsService TransfersRequestsService;
        private readonly NotificationsService NotificationsService;
        private readonly SapService SapService;

        public TransfersRequestsController(
            TransfersRequestsService transfersRequestsService,
            NotificationsService notificationsService,
            SapService sapService)
        {
            TransfersRequestsService = transfersRequestsService;
            NotificationsService = notificationsService;
            SapService = sapService;
        }

        /// <summary>
        /// Metodo para registrar nuevas solicitudes de traslado en SAP
        /// </summary>
        /// <response code="200">La solicitud de traslado fue registrada correctamente</response>
        /// <response code="400">Los datos de entrada no son validos</response>
        /// <response code="500">Ocurrio un error y la solicitud de traslado no puede ser registrada</response>
        /// <param name="Transfer">Datos de la solicitud de traslado</param>
        [HttpPost]
        [ProducesResponseType(typeof(TransferRequestSuccessResponse), 200)]
        [ProducesResponseType(typeof(TransferRequestErrorResponse), 500)]
        [ProducesResponseType(typeof(TransferRequestBadResponse), 400)]
        async public Task<IActionResult> SaveTransferRequest([FromBody] TransferRequest Transfer)
        {
            try
            {
                if (string.IsNullOrEmpty(Transfer.CodSF) ||
                    string.IsNullOrEmpty(Transfer.CardCode) ||
                    Transfer.DocDate == null ||
                    Transfer.DocDueDate == null ||
                    Transfer.TaxDate == null ||
                    string.IsNullOrEmpty(Transfer.AlmacenOrigen) ||
                    string.IsNullOrEmpty(Transfer.AlmacenDestino) ||
                    (Transfer.Detail == null || Transfer.Detail?.Count == 0))
                {
                    return new HttpResponse
                        .Error(
                            400,
                            "Los datos de entrada no son validos",
                            "Uno de los campos requeridos no tienen un valor asignado")
                        .Send();
                }

                TransferRequest transferInDb = await TransfersRequestsService.GetTransferRequest(Transfer.CodSF);
                Notification notification = null;

                if (transferInDb != null)
                {
                    notification = await NotificationsService.GetNotification(transferInDb.Id.Value, "Solicitud de traslado");
                    transferInDb.Detail = await TransfersRequestsService.GetTransferRequestDetail(transferInDb.Id.Value);

                    if (notification != null)
                    {
                        if (notification.DocNum.HasValue)
                        {
                            try
                            {
                                SapService.UpdateTransferRequest(notification.DocEntry.Value, Transfer);
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
                                    Operation = "Actualizando solicitud de traslado en SAP",
                                    Status = "ERROR",
                                });

                                notification.Status = "Fallida";
                                await NotificationsService.UpdateNotification(notification);

                                return new HttpResponse
                                    .Error(500, "No logro actualizarse la solicitud de traslado en SAP", ex.Message)
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
                            await TransfersRequestsService.DeleteTransferRequest(transferInDb.Id.Value);

                            string Anexo = Transfer.Anexo;
                            Transfer.Anexo = null;
                            Transfer.Id = await TransfersRequestsService.SaveTransferRequest(Transfer);
                            Transfer.Anexo = Anexo;

                            foreach (var line in Transfer.Detail)
                            {
                                line.TransferId = Transfer.Id.Value;
                                line.Id = await TransfersRequestsService.SaveTransferRequestsLine(line);
                            }

                            transferInDb = Transfer;

                            notification.RefId = Transfer.Id.Value;
                            await NotificationsService.UpdateNotification(notification);
                        }
                    }
                }

                long transferId = 0;

                if (transferInDb == null)
                {
                    string Anexo = Transfer.Anexo;
                    Transfer.Anexo = null;
                    Transfer.Id = await TransfersRequestsService.SaveTransferRequest(Transfer);
                    Transfer.Anexo = Anexo;

                    foreach (var line in Transfer.Detail)
                    {
                        line.TransferId = Transfer.Id;
                        line.Id = await TransfersRequestsService.SaveTransferRequestsLine(line);
                    }

                    transferInDb = Transfer;
                    transferId = transferInDb.Id.Value;
                }
                else
                {
                    transferId = transferInDb.Id.Value;
                }

                if (notification == null)
                {
                    await NotificationsService.AddNotification(new Notification
                    {
                        CreateTime = DateTime.Now,
                        Stage = "Registrando datos solicitud de traslado",
                        Status = "Pendiente",
                        Steps = 1,
                        RefType = "Solicitud de traslado",
                        RefId = transferId,
                        FinishTime = null,
                        DocEntry = null,
                        DocNum = null
                    });

                    notification = await NotificationsService.GetNotification(transferInDb.Id.Value, "Solicitud de traslado");
                }
                else
                {
                    notification = await NotificationsService.GetNotification(transferInDb.Id.Value, "Solicitud de traslado");
                    notification.Steps += 1;
                    await NotificationsService.UpdateNotification(notification);
                }

                Tuple<int, int> Values = null;

                try
                {
                    Values = SapService.CreateTransferRequest(Transfer);
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
                        Operation = "Creando solicitud de traslado en SAP",
                        Status = "ERROR",
                    });

                    notification.Status = "Fallida";
                    await NotificationsService.UpdateNotification(notification);

                    return new HttpResponse
                        .Error(500, "No logro registrarse la solicitud de traslado en SAP", ex.Message)
                        .Send();
                }

                return new HttpResponse.Response<int>
                {
                    Data = Values.Item2,
                    Error = null,
                    Status = 200,
                    Message = "Numero documento solicitud de traslado en SAP"
                }.Send();
            }
            catch (Exception err)
            {
                return new HttpResponse
                    .Error(500, "No logro registrarse la solicitud de traslado en SAP", $"{err.Message}\n{err.StackTrace}")
                    .Send();
            }
        }
    }
}
