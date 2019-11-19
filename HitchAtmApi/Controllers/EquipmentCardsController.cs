using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using HitchAtmApi.Models;
using HitchAtmApi.ViewModel;
using HitchAtmApi.Lib;
using HitchAtmApi.SwaggerExamples.Responses;

namespace HitchAtmApi.Controllers
{
    [Route("hitch/equipment_cards")]
    [EnableCors("AllowAllOrigins")]
    [Produces("application/json")]
    [ApiController]
    public class EquipmentCardsController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly SapService SapService;

        public EquipmentCardsController(
            IConfiguration configuration,
            SapService sapService)
        {
            Configuration = configuration;
            SapService = sapService;
        }

        /// <summary>
        /// Metodo para actualizar una tarjeta de equipo
        /// </summary>
        /// <response code="200">Tarjeta de equipo actualizada</response>
        /// <response code="500">Ocurrio un error y no es posible actualizar la tarjeta</response>
        /// <param name="input">Codigo de la tarjeta y datos a modificar</param>
        [HttpPut]
        [ProducesResponseType(typeof(EquipmentCardSuccessResponse), 200)]
        [ProducesResponseType(typeof(EquipmentCardErrorResponse), 500)]
        public IActionResult UpdateCard([FromBody] CardInput input)
        {
            try
            {
                SapService.UpdateCard(
                    input.insID, input.Status, input.Address);

                return new HttpResponse.Response<int>
                {
                    Data = input.insID,
                    Error = null,
                    Status = 200,
                    Message = "Tarjeta de equipo actualizada"
                }.Send();
            }
            catch (Exception ex)
            {
                return new HttpResponse.Error(
                    500, "Fallo la actualizacion de la tarjeta de equipo", ex.Message).Send();
            }
        }
    }
}
