using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HitchAtmApi.Models
{
    /// <summary>Orden de compra</summary>
    public class PurchaseOrder
    {
        [JsonIgnore]
        public long? Id { get; set; }

        /// <summary>Codigo del socio de negocio</summary>
        [JsonProperty("CardCode")]
        [Required]
        public string CardCode { get; set; }

        /// <summary>Fecha de contabilizacion</summary>
        [JsonProperty("DocDate")]
        [Required]
        public DateTime DocDate { get; set; }

        /// <summary>Fecha de entrega</summary>
        [JsonProperty("DocDueDate")]
        [Required]
        public DateTime DocDueDate { get; set; }

        /// <summary>Fecha de documento</summary>
        [JsonProperty("TaxDate")]
        [Required]
        public DateTime TaxDate { get; set; }

        /// <summary>Numero de serie documento</summary>
        [JsonProperty("Serie")]
        public int? Serie { get; set; }

        /// <summary>Campo de usuario: Tipo Solicitud</summary>
        [JsonProperty("TipoSolic")]
        public string TipoSolic { get; set; }

        /// <summary>Campo de usuario: Fecha Salida</summary>
        [JsonProperty("FechaSalida")]
        public DateTime? FechaSalida { get; set; }

        /// <summary>Campo de usuario: Fecha Llegada</summary>
        [JsonProperty("FechaLlegada")]
        public DateTime? FechaLlegada { get; set; }

        /// <summary>Campo de usuario: Numero de Visitas</summary>
        [JsonProperty("NumVisitas")]
        public int? NumVisitas { get; set; }

        /// <summary>Campo de usuario: # Oportunidad de Venta</summary>
        [JsonProperty("OportVentas")]
        public string OportVentas { get; set; }

        /// <summary>Campo de usuario: Observaciones Cobertura</summary>
        [JsonProperty("ObsCobertura")]
        public string ObsCobertura { get; set; }

        /// <summary>Campo de usuario: Hora Salida Santiago</summary>
        [JsonProperty("HoraSalidaStgo")]
        public string HoraSalidaStgo { get; set; }

        /// <summary>Campo de usuario: Hora ultima visita</summary>
        [JsonProperty("HoraUltimaVisita")]
        public string HoraUltimaVisita { get; set; }

        /// <summary>Campo de usuario: Ciudades Destino</summary>
        [JsonProperty("CiudadesDest")]
        public string CiudadesDest { get; set; }

        /// <summary>Campo de usuario: OC Origen</summary>
        [JsonProperty("OCOrigen")]
        public int? OCOrigen { get; set; }

        /// <summary>Campo de usuario: % Descuento</summary>
        [JsonProperty("Discount")]
        public string Discount { get; set; }

        /// <summary>Codigo salesforce de la orden de compra</summary>
        [JsonProperty("CodSF")]
        [Required]
        public string CodSF { get; set; }

        /// <summary>Contenido orden de compra</summary>
        [JsonProperty("Detail")]
        [Required]
        public List<PurchaseOrderDetail> Detail { get; set; }
    }

    /// <summary>Linea de contenido de orden de compra</summary>
    public class PurchaseOrderDetail
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

        /// <summary>Codigo linea</summary>
        [JsonProperty("Dim1")]
        public string Dim1 { get; set; }

        /// <summary>Codigo area</summary>
        [JsonProperty("Dim2")]
        public string Dim2 { get; set; }

        /// <summary>Codigo Unidad de negocio</summary>
        [JsonProperty("Dim3")]
        public string Dim3 { get; set; }

        [JsonIgnore]
        public long? OrderId { get; set; }
    }
}
