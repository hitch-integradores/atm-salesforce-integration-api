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
    [Route("hitch/products")]
    [EnableCors("AllowAllOrigins")]
    [Produces("application/json")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly SapService SapService;

        public ProductsController(
            IConfiguration configuration,
            SapService sapService)
        {
            Configuration = configuration;
            SapService = sapService;
        }

        /// <summary>
        /// Metodo para obtener datos basicos de un articulo
        /// </summary>
        /// <response code="200">Datos del articulo</response>
        /// <response code="404">No se encontro el articulo</response>
        /// <response code="500">Ocurrio un error y no es posible obtener los datos</response>
        /// <param name="input">Codigo del articulo</param>
        [HttpPost]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(ProductErrorResponse), 500)]
        [ProducesResponseType(typeof(ProductNotFoundResponse), 404)]
        public IActionResult GetProducts([FromBody] ProductInput input)
        {
            try
            {
                Product product = SapService.GetProduct(input.ItemCode);

                if (product == null)
                {
                    return new HttpResponse.Error(
                        404, "No encontrado", "No existe un producto con el codigo = " + input.ItemCode).Send();
                }

                return new JsonResult(product);
            }
            catch (Exception ex)
            {
                return new HttpResponse.Error(
                    500, "Fallo la obtencion del producto", ex.Message).Send();
            }
        }
    }
}
