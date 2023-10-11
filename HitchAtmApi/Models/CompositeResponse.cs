using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HitchAtmApi.Models
{
    public class CompositeAddressResponse
    {
        public List<compositeResponse> compositeResponse{ get; set; }
    }

    public class compositeResponse
    {
        public dynamic body { get; set; }
        public dynamic httpHeaders { get; set; }
        public int httpStatusCode { get; set; }
        public string referenceId { get; set; }
    }

    public class AddressBody
    {
        public attributes attributes { get; set; }
        public string Id { get; set; }
        public bool? IsDeleted { get; set; }
        public string Name { get; set; }
        public string CurrencyIsoCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? SystemModstamp { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public DateTime? LastReferencedDate { get; set; }
        public string Cuenta__c { get; set; }
        public string Buscador__c { get; set; }
        public string Calle__c { get; set; }
        public string Ciudad__c { get; set; }
        public string Comuna__c { get; set; }
        public string Comuna_texto__c { get; set; }
        public string Id_externo__c { get; set; }
        public string Indicador_de_impuestos__c { get; set; }
        public string Pais__c { get; set; }
        public string Region__c { get; set; }
        public string Tipo__c { get; set; }
        public string Autonumerico__c { get; set; }
    }

    public class CountyBody
    {
        public attributes attributes { get; set; }
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public bool? IsDeleted { get; set; }
        public string Name { get; set; }
        public string CurrencyIsoCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? SystemModstamp { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public DateTime? LastReferencedDate { get; set; }
        public string Codigo_SAP__c { get; set; }
        public string Region__c { get; set; }
    }

    public class StateBody
    {
        public attributes attributes { get; set; }
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public bool? IsDeleted { get; set; }
        public string Name { get; set; }
        public string CurrencyIsoCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? SystemModstamp { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public DateTime? LastReferencedDate { get; set; }
        public string Codigo_SAP__c { get; set; }
        public string Pais__c { get; set; }
    }

    public class attributes
    {
        public string type { get; set; }
        public string url { get; set; }

    }
}
