using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HitchAtmApi.Models
{
    /// <summary>Solicitud de traslado</summary>
    public class TransferRequest
    {
        [JsonIgnore]
        public long? Id { get; set; }

        /// <summary>Codigo del socio de negocio</summary>
        [JsonProperty("CardCode")]
        [Required]
        public string CardCode { get; set; }

        /// <summary>Codigo persona de contacto</summary>
        [JsonProperty("Contacto")]
        public int? Contacto { get; set; }

        /// <summary>Fecha de contabilizacion</summary>
        [JsonProperty("DocDate")]
        [Required]
        public DateTime DocDate { get; set; }

        /// <summary>Fecha de vencimiento</summary>
        [JsonProperty("DocDueDate")]
        [Required]
        public DateTime DocDueDate { get; set; }

        /// <summary>Fecha de documento</summary>
        [JsonProperty("TaxDate")]
        [Required]
        public DateTime TaxDate { get; set; }

        /// <summary>Almacen de origen</summary>
        [JsonProperty("AlmacenOrigen")]
        [Required]
        public string AlmacenOrigen { get; set; }

        /// <summary>Almacen de destino</summary>
        [JsonProperty("AlmacenDestino")]
        [Required]
        public string AlmacenDestino { get; set; }

        /// <summary>Codigo solicitante</summary>
        [JsonProperty("SlpCode")]
        public int? SlpCode { get; set; }

        /// <summary>Comentario</summary>
        [JsonProperty("ImlMemo")]
        public string ImlMeno { get; set; }

        /// <summary>Campo de usuario: Tipo</summary>
        [JsonProperty("TipoPD")]
        public string TipoPD { get; set; }

        /// <summary>Campo de usuario: Codigo Cliente</summary>
        [JsonProperty("CodeSN")]
        public string CodeSN { get; set; }

        /// <summary>Campo de usuario: Razon Social</summary>
        [JsonProperty("RazonSocial")]
        public string RazonSocial { get; set; }

        /// <summary>Campo de usuario: Fecha Inicio</summary>
        [JsonProperty("DateStart")]
        public DateTime? DateStart { get; set; }

        /// <summary>Campo de usuario: Fecha Termino</summary>
        [JsonProperty("DateEnd")]
        public DateTime? DateEnd { get; set; }

        /// <summary>Campo de usuario: Ubicacion</summary>
        [JsonProperty("Ubicacion")]
        public string Ubicacion { get; set; }

        /// <summary>Campo de usuario: Observaciones</summary>
        [JsonProperty("Observaciones")]
        public string Observaciones { get; set; }

        /// <summary>Campo de usuario: Oportunidad de Venta</summary>
        [JsonProperty("NumOVSF")]
        public int? NumOVSF { get; set; }

        /// <summary>Campo de usuario: Llamada de Servicio</summary>
        [JsonProperty("LlamadaServ")]
        public int? LlamadaServ { get; set; }

        /// <summary>Anexo</summary>
        /// <remarks>PDF codificado en base64</remarks>
        [JsonProperty("Anexo")]
        public string Anexo { get; set; }

        /// <summary>Codigo salesforce solicitud de traslado</summary>
        [JsonProperty("CodSF")]
        [Required]
        public string CodSF { get; set; }

        /// <summary>Contenido solicitud de traslado</summary>
        [JsonProperty("Detail")]
        [Required]
        public List<TransferRequestDetail> Detail { get; set; }
    }

    /// <summary>Linea de contenido de solicitud de traslado</summary>
    public class TransferRequestDetail
    {
        [JsonIgnore]
        public long? Id { get; set; }

        /// <summary>Codigo articulo</summary>
        [JsonProperty("ItemCode")]
        public string ItemCode { get; set; }

        /// <summary>Almacen de origen</summary>
        [JsonProperty("Almacen")]
        public string Almacen { get; set; }

        [JsonProperty("AlmacenDest")]
        /// <summary>Almacen de destino</summary>
        public string AlmacenDest { get; set; }

        /// <summary>Cantidad articulo</summary>
        [JsonProperty("Quantity")]
        public int Quantity { get; set; }

        [JsonIgnore]
        public long? TransferId { get; set; }
    }
}
