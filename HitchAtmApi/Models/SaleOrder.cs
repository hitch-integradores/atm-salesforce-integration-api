using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HitchAtmApi.Models
{
    /// <summary>Orden de venta</summary>
    public class SaleOrder
    {
        [JsonIgnore]
        public long? Id { get; set; }

        /// <summary>Codigo del socio de negocio</summary>
        [JsonProperty("CardCode")]
        [Required]
        public string CardCode { get; set; }

        /// <summary>Codigo persona de contacto</summary>
        [JsonProperty("CNTCCode")]
        public int? CNTCCode { get; set; }

        /// <summary>Fecha de contabilizacion</summary>
        [JsonProperty("DocDate")]
        [Required]
        public DateTime? DocDate { get; set; }

        /// <summary>Fecha de entrega comprometida</summary>
        [JsonProperty("DocDueDate")]
        [Required]
        public DateTime? DocDueDate { get; set; }

        /// <summary>Fecha del documento</summary>
        [JsonProperty("TaxDate")]
        [Required]
        public DateTime? TaxDate { get; set; }

        /// <summary>Campo de usuario: % Descuento</summary>
        [JsonProperty("DescuentoTotal")]
        public string DescuentoTotal { get; set; }

        [JsonProperty("DescuentoCabecera")]
        public double? DescuentoCabecera { get; set; }

        /// <summary>Codigo del encargado de ventas</summary>
        [JsonProperty("Vendedor")]
        public int? Vendedor { get; set; }

        /// <summary>ID direccion de despacho</summary>
        [JsonProperty("ShipToCode")]
        public string ShipToCode { get; set; }

        /// <summary>ID direccion de facturacion</summary>
        [JsonProperty("PayToCode")]
        public string PayToCode { get; set; }


        /// <summary>Codigo de la comuna</summary>
        [JsonProperty("CountyCode")]
        public string CountyCode { get; set; }

        /// <summary>Codigo de la region</summary>
        [JsonProperty("StateCode")]
        public string StateCode { get; set; }

        /// <summary>Codigo del pais</summary>
        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }


        /// <summary>Codigo condicion de pago</summary>
        [JsonProperty("PaymentCondition")]
        public int? PaymentCondition { get; set; }

        /// <summary>Entrega parcial (Si o No)</summary>
        [JsonProperty("PartSuply")]
        [Required]
        public bool? PartSuply { get; set; }

        /// <summary>Campo de usuario: Zona</summary>
        [JsonProperty("Project")]
        public string Project { get; set; }

        /// <summary>Campo de usuario: Capacitacion Requerida</summary>
        [JsonProperty("CapacitacionReq")]
        public string CapacitacionReq { get; set; }

        /// <summary>Campo de usuario: Instalacion Requerida</summary>
        [JsonProperty("InstalacionReq")]
        public string InstalacionReq { get; set; }

        /// <summary>Campo de usuario: Garantia Pactada</summary>
        [JsonProperty("GarantiaPactada")]
        public string GarantiaPactada { get; set; }

        /// <summary>Campo de usuario: Mantenimiento Preventivo</summary>
        [JsonProperty("MantPreventivo")]
        public string MantPreventivo { get; set; }

        /// <summary>Campo de usuario: # visitas x año Gar.</summary>
        [JsonProperty("NumVisitasAnoGarantia")]
        public int? NumVisitasAnoGarantia { get; set; }

        /// <summary>Campo de usuario: O/C Cliente</summary>
        [JsonProperty("OCcliente")]
        public string OCcliente { get; set; }

        /// <summary>Campo de usuario: Orden de Compra</summary>
        /// <remarks>PDF codificado en base64</remarks>
        [JsonProperty("DocOCCliente")]
        public string DocOCCliente { get; set; }

        /// <summary>Extension archivo orden de compra</summary>
        /// <remarks>Extension archivo codificado en base64</remarks>
        [JsonProperty("DocOCExt")]
        public string DocOCExt { get; set; }

        /// <summary>Campo de usuario: ¿Existen Multas?</summary>
        [JsonProperty("ExistenMultas")]
        public string ExistenMultas { get; set; }

        /// <summary>Campo de usuario: Fecha O/C Cliente</summary>
        [JsonProperty("DateOCCLiente")]
        public DateTime? DateOCCLiente { get; set; }

        /// <summary>Campo de usuario: Fecha recep. O/C Cliente</summary>
        [JsonProperty("DateRecepcionOC")]
        public DateTime? DateRecepcionOC { get; set; }

        /// <summary>Campo de usuario: Fecha de Entrega</summary>
        [JsonProperty("FechaEntrega")]
        public DateTime? FechaEntrega { get; set; }

        /// <summary>Campo de usuario: Contacto Socio Negocio</summary>
        [JsonProperty("ContactSN")]
        public string ContactSN { get; set; }

        /// <summary>Campo de usuario: Rut Socio Negocio</summary>
        [JsonProperty("RutSN")]
        public string RutSN { get; set; }

        /// <summary>Campo de usuario: Nombre Socio de Negocio</summary>
        [JsonProperty("NomSN")]
        public string NomSN { get; set; }

        /// <summary>Campo de usuario: Cotizacion Provisoria</summary>
        [JsonProperty("NumCotizacionProv")]
        public string NumCotizacionProv { get; set; }

        /// <summary>Campo de usuario: Canc. x falta de stock (Si o No)</summary>
        [JsonProperty("CancFaltaStock")]
        public bool? CancFaltaStock { get; set; }

        /// <summary>Campo de usuario: ¿Leasing ATM?</summary>
        [JsonProperty("LeasingATM")]
        public bool? LeasingATM { get; set; }

        /// <summary>Campo de usuario: Direccion entrega Factura</summary>
        [JsonProperty("DirecEntregaFactura")]
        public string DirecEntregaFactura { get; set; }

        /// <summary>Codigo salesforce de la orden de venta</summary>
        [JsonProperty("CodSF")]
        [Required]
        public string CodSF { get; set; }

        /// <summary>Comentario de la orden de venta</summary>
        [JsonProperty("Comments")]
        public string Comments { get; set; }

        /// <summary>Campo de usuario: Documento de Despacho</summary>
        [JsonProperty("DocumentoDespacho")]
        public string DocumentoDespacho { get; set; }

        /// <summary>Campo de usuario: Fecha Probable Facturacion</summary>
        [JsonProperty("FechaProbableFacturacion")]
        public DateTime? FechaProbableFacturacion { get; set; }

        /// <summary>Campo de usuario: ¿Exigencia de Backup?</summary>
        [JsonProperty("ExigenciaBackup")]
        public bool? ExigenciaBackup { get; set; }

        /// <summary>ID oportunidad</summary>
        [JsonProperty("IdOportunidad")]
        public string IdOportunidad { get; set; }

        /// <summary>Contenido orden de venta</summary>
        [JsonProperty("Detail")]
        [Required]
        public List<SaleOrderDetail> Detail { get; set; }

        /// <summary>Archivos o adjuntos de la orden de venta</summary>
        [JsonProperty("Attachments")]
        public List<SaleOrderAttachment> Attachments { get; set; }
    }

    /// <summary>Linea de contenido de orden de venta</summary>
    public class SaleOrderDetail
    {
        [JsonIgnore]
        public long? Id { get; set; }

        /// <summary>Codigo articulo</summary>
        [JsonProperty("ItemCode")]
        [Required]
        public string ItemCode { get; set; }

        /// <summary>Cantidad articulo</summary>
        [JsonProperty("Quantity")]
        [Required]
        public int Quantity { get; set; }

        /// <summary>Campo de usuario: % Descuento</summary>
        [JsonProperty("Descuento")]
        public string Descuento { get; set; }

        /// <summary>Precio por unidad del articulo</summary>
        [JsonProperty("UnitPrice")]
        public double? UnitPrice { get; set; }

        /// <summary>Almacen stock del articulo</summary>
        [JsonProperty("Almacen")]
        public string Almacen { get; set; }

        /// <summary>Fecha de entrega del articulo</summary>
        [JsonProperty("DateEntrega")]
        public DateTime? DateEntrega { get; set; }

        /// <summary>NO ASIGNADO</summary>
        [JsonProperty("Vendedor")]
        public int? Vendedor { get; set; }

        /// <summary>NO ASIGNADO</summary>
        [JsonProperty("Comments")]
        public string Comments { get; set; }

        /// <summary>Descripcion del articulo</summary>
        [JsonProperty("Description")]
        public string Description { get; set; }

        /// <summary>ID de salesforce del articulo</summary>
        [JsonProperty("IDSF")]
        public string IDSF { get; set; }

        /// <summary>Codigo linea</summary>
        [JsonProperty("Dim1")]
        public string Dim1 { get; set; }

        /// <summary>Codigo area</summary>
        [JsonProperty("Dim2")]
        public string Dim2 { get; set; }

        /// <summary>Codigo Unidad de negocio</summary>
        [JsonProperty("Dim3")]
        public string Dim3 { get; set; }

        /// <summary>Codigo Zona</summary>
        [JsonProperty("Zona")]
        public string Zona { get; set; }

        /// <summary>Impuesto</summary>
        [JsonProperty("TaxCode")]
        public string TaxCode { get; set; }

        /// <summary>Moneda</summary>
        [JsonProperty("CurrencyCode")]
        public string CurrencyCode { get; set; }

        [JsonIgnore]
        public long? OrderId { get; set; }
    }

    public class SaleOrderAttachment
    {
        /// <summary>Contenido del archivo</summary>
        /// <remarks>archivo codificado en base64</remarks>
        [JsonProperty("Content")]
        public string Content { get; set; }

        /// <summary>Nombre del archivo</summary>
        [JsonProperty("Filename")]
        public string Filename { get; set; }

        /// <summary>Descripcion o texto indicando que es el archivo</summary>
        [JsonProperty("Text")]
        public string Text { get; set; }

        /// <summary>Fecha del archivo</summary>
        [JsonProperty("Date")]
        public DateTime? Date { get; set; }
    }
}
