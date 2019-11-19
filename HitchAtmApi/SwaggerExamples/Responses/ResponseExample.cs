using Swashbuckle.AspNetCore.Filters;
using HitchAtmApi.Models;

namespace HitchAtmApi.SwaggerExamples.Responses
{
    public class CustomerErrorResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class CustomerNotFoundResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class ProductErrorResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class ProductNotFoundResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class EquipmentCardSuccessResponse
    {
        public int Data { get; set; }
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class EquipmentCardErrorResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class SaleOrderSuccessResponse
    {
        public int Data { get; set; }
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class SaleOrderErrorResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class SaleOrderBadResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class PurchaseOrderSuccessResponse
    {
        public int Data { get; set; }
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class PurchaseOrderErrorResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class PurchaseOrderBadResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class TransferRequestSuccessResponse
    {
        public int Data { get; set; }
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class TransferRequestErrorResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class TransferRequestBadResponse
    {
        public string Error { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class GetCustomerSuccessResponseExample : IExamplesProvider<Customer>
    {
        public Customer GetExamples()
        {
            return new Customer
            {
                CardCode = "9999111P",
                CardName = "Maria Francisca Redard",
                CardFName = "+ Ideas",
                LicTradNum = "9999111-9"
            };
        }
    }

    public class GetCustomerErrorResponseExample : IExamplesProvider<CustomerErrorResponse>
    {
        public CustomerErrorResponse GetExamples()
        {
            return new CustomerErrorResponse
            {
                Error = "Detalle del error",
                Status = 500,
                Message = "Fallo la obtencion del cliente"
            };
        }
    }

    public class GetCustomerNotFoundResponseExample : IExamplesProvider<CustomerNotFoundResponse>
    {
        public CustomerNotFoundResponse GetExamples()
        {
            return new CustomerNotFoundResponse
            {
                Error = "No existe un cliente con un LicTradNum = 11111111-1",
                Status = 404,
                Message = "No encontrado"
            };
        }
    }

    public class GetProductSuccessResponseExample : IExamplesProvider<Product>
    {
        public Product GetExamples()
        {
            return new Product
            {
                ItemCode = "NI40370-M094",
                ItemName = "Nidek Pliable Cup (Red)",
                StatusProduct = 1
            };
        }
    }

    public class GetProductErrorResponseExample : IExamplesProvider<ProductErrorResponse>
    {
        public ProductErrorResponse GetExamples()
        {
            return new ProductErrorResponse
            {
                Error = "Detalle de error",
                Status = 500,
                Message = "Fallo la obtencion del producto"
            };
        }
    }

    public class GetProductNotFoundResponseExample : IExamplesProvider<ProductNotFoundResponse>
    {
        public ProductNotFoundResponse GetExamples()
        {
            return new ProductNotFoundResponse
            {
                Error = "No existe un producto con el codigo = NI40370-M094",
                Status = 404,
                Message = "No encontrado"
            };
        }
    }

    public class UpdateEquipmentSuccessResponseExample : IExamplesProvider<EquipmentCardSuccessResponse>
    {
        public EquipmentCardSuccessResponse GetExamples()
        {
            return new EquipmentCardSuccessResponse
            {
                Data = 5187,
                Error = null,
                Status = 200,
                Message = "Tarjeta de equipo actualizada"
            };
        }
    }

    public class UpdateEquipmentCardErrorResponseExample : IExamplesProvider<EquipmentCardErrorResponse>
    {
        public EquipmentCardErrorResponse GetExamples()
        {
            return new EquipmentCardErrorResponse
            {
                Error = "Detalle del error",
                Status = 500,
                Message = "Fallo la actualizacion de la tarjeta de equipo"
            };
        }
    }

    public class CreateSaleOrderSuccessResponseExample : IExamplesProvider<SaleOrderSuccessResponse>
    {
        public SaleOrderSuccessResponse GetExamples()
        {
            return new SaleOrderSuccessResponse
            {
                Data = 5434,
                Status = 200,
                Message = "Numero documento orden de venta en SAP"
            };
        }
    }

    public class CreateSaleOrderErrorResponseExample : IExamplesProvider<SaleOrderErrorResponse>
    {
        public SaleOrderErrorResponse GetExamples()
        {
            return new SaleOrderErrorResponse
            {
                Error = "Error registrando orden de venta: Actualice los tipos de cambio",
                Status = 500,
                Message = "No logro registrarse la orden de venta en SAP"
            };
        }
    }

    public class CreateSaleOrderBadResponseExample : IExamplesProvider<SaleOrderBadResponse>
    {
        public SaleOrderBadResponse GetExamples()
        {
            return new SaleOrderBadResponse
            {
                Error = "Uno de los campos requeridos no tienen un valor asignado",
                Status = 400,
                Message = "Los datos de entrada no son validos"
            };
        }
    }

    public class CreatePurchaseOrderSuccessResponseExample : IExamplesProvider<PurchaseOrderSuccessResponse>
    {
        public PurchaseOrderSuccessResponse GetExamples()
        {
            return new PurchaseOrderSuccessResponse
            {
                Data = 5434,
                Status = 200,
                Message = "Numero documento orden de compra en SAP"
            };
        }
    }

    public class CreatePurchaseOrderErrorResponseExample : IExamplesProvider<PurchaseOrderErrorResponse>
    {
        public PurchaseOrderErrorResponse GetExamples()
        {
            return new PurchaseOrderErrorResponse
            {
                Error = "Error registrando orden de compra: Actualice los tipos de cambio",
                Status = 500,
                Message = "No logro registrarse la orden de compra en SAP"
            };
        }
    }

    public class CreatePurchaseOrderBadResponseExample : IExamplesProvider<PurchaseOrderBadResponse>
    {
        public PurchaseOrderBadResponse GetExamples()
        {
            return new PurchaseOrderBadResponse
            {
                Error = "Uno de los campos requeridos no tienen un valor asignado",
                Status = 400,
                Message = "Los datos de entrada no son validos"
            };
        }
    }

    public class CreateTransferRequestSuccessResponseExample : IExamplesProvider<TransferRequestSuccessResponse>
    {
        public TransferRequestSuccessResponse GetExamples()
        {
            return new TransferRequestSuccessResponse
            {
                Data = 5434,
                Status = 200,
                Message = "Numero documento solicitud de traslado en SAP"
            };
        }
    }

    public class CreateTransferRequestErrorResponseExample : IExamplesProvider<TransferRequestErrorResponse>
    {
        public TransferRequestErrorResponse GetExamples()
        {
            return new TransferRequestErrorResponse
            {
                Error = "Error registrando solicitud de traslado: El articulo TEST01 no existe",
                Status = 500,
                Message = "No logro registrarse la solicitud de traslado en SAP"
            };
        }
    }

    public class CreateTransferRequestBadResponseExample : IExamplesProvider<TransferRequestBadResponse>
    {
        public TransferRequestBadResponse GetExamples()
        {
            return new TransferRequestBadResponse
            {
                Error = "Uno de los campos requeridos no tienen un valor asignado",
                Status = 400,
                Message = "Los datos de entrada no son validos"
            };
        }
    }
}
