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
    [Route("hitch/customers")]
    [EnableCors("AllowAllOrigins")]
    [Produces("application/json")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly SapService SapService;

        public CustomersController(
            IConfiguration configuration,
            SapService sapService)
        {
            Configuration = configuration;
            SapService = sapService;
        }

        /// <summary>
        /// Metodo para obtener datos basicos de un socio de negocio
        /// </summary>
        /// <response code="200">Datos del socio de negocio</response>
        /// <response code="404">No se encontro el socio de negocio</response>
        /// <response code="500">Ocurrio un error y no es posible obtener los datos</response>
        /// <param name="input">Rut del socio de negocio</param>
        [HttpPost]
        [ProducesResponseType(typeof(Customer), 200)]
        [ProducesResponseType(typeof(CustomerErrorResponse), 500)]
        [ProducesResponseType(typeof(CustomerNotFoundResponse), 404)]
        public IActionResult GetCustomer([FromBody] CustomerInput input)
        {
            try
            {
                Customer customer = SapService.GetCustomer(input.LicTradNum);

                if (customer == null)
                {
                    return new HttpResponse.Error(
                        404, "No encontrado", "No existe un cliente con un LicTradNum = " + input.LicTradNum).Send();
                }

                return new JsonResult(customer);
            }
            catch (Exception ex)
            {
                return new HttpResponse.Error(
                    500, "Fallo la obtencion del cliente", ex.Message).Send();
            }
        }
    }
}
