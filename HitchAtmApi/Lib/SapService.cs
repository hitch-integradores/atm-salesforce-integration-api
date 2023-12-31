﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using HitchAtmApi.Models;
using HitchSapB1Lib;
using HitchSapB1Lib.Operations;
using HitchSapB1Lib.Enums;
using HitchSapB1Lib.Objects.Marketing;
using HitchSapB1Lib.Objects.Definition;
using HitchSapB1Lib.Objects.Inventory;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace HitchAtmApi.Lib
{
    public class SapService
    {
        HitchSapB1Lib.ConnectionParameters DefaultConnectionParameters;

        public SapService(
            IConfiguration configuration)
        {
            SapConnectionParameters sapConnectionParameters = new SapConnectionParameters
            {
                DatabaseType = configuration.GetValue<string>("Sap:DatabaseType"),
                License = configuration.GetValue<string>("Sap:License"),
                Password = configuration.GetValue<string>("Sap:Password"),
                Server = configuration.GetValue<string>("Sap:Server"),
                ServerDatabase = configuration.GetValue<string>("Sap:ServerDatabase"),
                ServerPassword = configuration.GetValue<string>("Sap:ServerPassword"),
                ServerUser = configuration.GetValue<string>("Sap:ServerUser"),
                Username = configuration.GetValue<string>("Sap:Username")
            };

            DefaultConnectionParameters = sapConnectionParameters.ToConnectionParameters();
        }

        public SapResponse CreateSaleOrder(HitchAtmApi.Models.SaleOrder Order)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                CreateSaleOrderOperation createSaleOrderOperation = new CreateSaleOrderOperation();

                HitchSapB1Lib.Objects.Marketing.SaleOrder SapOrder = new HitchSapB1Lib.Objects.Marketing.SaleOrder
                {
                    CustomerCode = Order.CardCode,
                    DocumentDate = Order.DocDate.Value,
                    DueDate = Order.DocDueDate.Value,
                    TaxDate = Order.TaxDate.Value,
                    Discount = Order.DescuentoCabecera,
                    PayToCode = Order.PayToCode, // ID EXTERNO DIRECCION SALESFORCE
                    ShipToCode = Order.ShipToCode, // ID EXTERNO DIRECCION SALESFORCE
                    PaymentCondition = Order.PaymentCondition,
                    IsPartialDelivery = Order.PartSuply.HasValue ? Order.PartSuply.Value : false,
                    CurrencySource = CurrencySource.Customer,
                    Comment = Order.Comments,
                    OwnerCode = null,
                    CustomerReferenceNumber = Order.IdOportunidad,
                    SalesEmployeeCode = Order.Vendedor,
                    ContactCode = Order.CNTCCode,
                    Serie = null,
                    UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                    Lines = Order.Detail.Select((det, i) =>
                    {
                        double? lineDiscount = 0;
                        double? linePrice = 0;
                        if (det.Descuento.HasValue == false)
                        {
                            lineDiscount = null;
                        }
                        else
                        {
                            try
                            {
                                lineDiscount = det.Descuento.Value;
                            }
                            catch (Exception)
                            {
                                throw new Exception($"El campo \"Descuento\" del item {i + 1} del campo \"Detail\" no es valido");
                            }
                        }
                        try
                        {
                            linePrice = Convert.ToDouble(det.UnitPrice);
                        }
                        catch (Exception)
                        {
                            throw new Exception($"El campo \"UnitPrice\" del item {i + 1} del campo \"Detail\" no es valido");
                        }
                        var line = new DocumentLine
                        {
                            ItemCode = det.ItemCode,
                            Quantity = det.Quantity,
                            Discount = lineDiscount,
                            Description = det.Description,
                            Warehouse = det.Almacen,
                            Price = linePrice,
                            DeliveryDate = det.DateEntrega,
                            Batchs = null,
                            CurrencyCode = string.IsNullOrEmpty(det.CurrencyCode) ? null : Utils.TranslateCurrencyCode(det.CurrencyCode),
                            UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                            Reference = null,
                            Series = null,
                            TaxCode = string.IsNullOrEmpty(det.TaxCode) ? "IVA" : Utils.TranslateTaxCode(det.TaxCode),
                            EmployeeCode = Order.Vendedor,
                            CostingCode1 = det.Dim1,
                            CostingCode2 = det.Dim2,
                            CostingCode3 = det.Dim3,
                            Project = det.Zona,
                            ApplyCommission = true
                        };
                        if (det.Descuento.HasValue)
                        {
                            line.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                            {
                                Name = "U_Dscto",
                                Value = det.Descuento.ToString()
                            });
                        }
                        return line;
                    }).ToList()
                };

                SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                {
                    Name = "U_SBOID",
                    Value = Order.CodSF ?? ""
                });

                if (string.IsNullOrEmpty(Order.CapacitacionReq) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCR",
                        Value = Order.CapacitacionReq
                    });
                }

                if (string.IsNullOrEmpty(Order.InstalacionReq) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOIR",
                        Value = Order.InstalacionReq
                    });
                }

                if (string.IsNullOrEmpty(Order.GarantiaPactada) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOGP",
                        Value = Order.GarantiaPactada
                    });
                }

                if (string.IsNullOrEmpty(Order.MantPreventivo) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOMP",
                        Value = Order.MantPreventivo
                    });
                }

                if (Order.NumVisitasAnoGarantia.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBON",
                        Value = Order.NumVisitasAnoGarantia.Value
                    });
                }

                if (Order.FechaEntrega.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOP",
                        Value = Order.FechaEntrega
                    });
                }

                if (string.IsNullOrEmpty(Order.OCcliente) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOC1",
                        Value = Order.OCcliente
                    });
                }

                if (string.IsNullOrEmpty(Order.DocOCCliente) == false)
                {
                    string fileLocation = Utils.SavePDFAttachment(
                        System.Guid.NewGuid().ToString() +
                        (string.IsNullOrEmpty(Order.DocOCExt) ?
                            ""
                            : ("." + Order.DocOCExt.Replace(".", ""))), Order.DocOCCliente);

                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOCC",
                        Value = fileLocation
                    });
                }

                if (string.IsNullOrEmpty(Order.ExistenMultas) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOMULTA",
                        Value = Order.ExistenMultas
                    });
                }

                if (string.IsNullOrEmpty(Order.Project) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOZONPD",
                        Value = Order.Project
                    });
                }

                if (Order.DateOCCLiente.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFOCC",
                        Value = Order.DateOCCLiente.Value
                    });
                }

                if (Order.DateRecepcionOC.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFRE",
                        Value = Order.DateRecepcionOC.Value
                    });
                }

                if (string.IsNullOrEmpty(Order.RutSN) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBORUT",
                        Value = Order.RutSN
                    });
                }

                if (string.IsNullOrEmpty(Order.NomSN) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBONOMSN",
                        Value = Order.NomSN
                    });
                }

                if (string.IsNullOrEmpty(Order.NumCotizacionProv) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCOTPROV",
                        Value = Order.NumCotizacionProv
                    });
                }

                if (Order.CancFaltaStock.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCFE",
                        Value = Order.CancFaltaStock.Value ? "S" : "N"
                    });
                }

                if (Order.LeasingATM.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_TIPOVTA",
                        Value = Order.LeasingATM.Value ? 1 : 0
                    });
                }

                if (string.IsNullOrEmpty(Order.DirecEntregaFactura) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBODIRENTFACT",
                        Value = Order.DirecEntregaFactura
                    });
                }

                if (Order.DescuentoTotal.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_DsctItmT",
                        Value = (Order.DescuentoTotal.ToString()) + " %"
                    });
                }

                if (string.IsNullOrEmpty(Order.DocumentoDespacho) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBODD",
                        Value = Order.DocumentoDespacho
                    });
                }

                if (Order.FechaProbableFacturacion.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFPF",
                        Value = Order.FechaProbableFacturacion
                    });
                }

                if (Order.ExigenciaBackup.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOBACKUP",
                        Value = Order.ExigenciaBackup.Value ? "Si" : "No"
                    });
                }

                SalesforceApi salesforceApi = new SalesforceApi(
                                    Utils.Credentials, Utils.SalesforceInstanceUrl, Utils.SalesforceApiVersion);
                var credentials = salesforceApi.GetCredentials(true);
                salesforceApi.Token = credentials.Token;
                dynamic Opportunity = salesforceApi.GetSalesForceAction("Opportunity", Order.CodSF);

                if (Opportunity == null && Opportunity?.Id == null)
                {
                    throw new Exception($"La Oportunidad con id {Order.CodSF} No existe o fue eliminada");
                }
                createSaleOrderOperation.SaleOrder = SapOrder;
                createSaleOrderOperation.PreExecutionHook = () =>
                {
                    try
                    {

                        dynamic customerResult = company.QueryOneResult<dynamic>($"SELECT TOP 1 CardCode FROM OCRD WHERE CardCode = '{SapOrder.CustomerCode}'");

                        if (customerResult == null)
                        {
                            dynamic customer = salesforceApi.GetSalesForceAction("Account", Opportunity.AccountId.ToString());

                            if (customer == null)
                            {
                                throw new Exception($"El cliente codigo {SapOrder.CustomerCode} no ha sido encontrado en SAP y tampoco en Salesforce");
                            }


                            int groupCode = 0;
                            dynamic groupResult = company.QueryOneResult<dynamic>($"SELECT OCRG.GroupCode FROM OCRG WHERE OCRG.GroupName = '{customer.Categoria_de_institucion__c}'");
                            if (groupResult != null)
                            {
                                groupCode = Convert.ToInt32(groupResult.GroupCode);
                            }

                            var createBusinessPartner = new CreateBusinessPartnerOperation();
                            createBusinessPartner.Company = company;
                            createBusinessPartner.Partner = new BusinessPartner
                            {
                                CardCode = SapOrder.CustomerCode,
                                CardName = customer.Name,
                                CardFName = customer.Nombre_de_fantasia__c,
                                Comments = customer.Comentarios__c,
                                CreditLine = customer.Linea_de_credito__c,
                                CurrencyIsoCode = Utils.TranslateCurrency((customer.CurrencyIsoCode as string)),
                                GroupCode = groupCode == 0 ? (int?)null : groupCode,
                                IsActive = customer.Cliente_bloqueado__c,
                                LicTradNum = customer.RUT__c,
                                PaymentCondition = customer.Codigo_de_condicion_de_pago__c != null ? Convert.ToInt32(customer.Codigo_de_condicion_de_pago__c) : null,
                                Phone = customer.Phone,
                                Mobile = customer.PersonMobilePhone,
                                Email = customer.PersonEmail,
                                OtherPhone = customer.PersonOtherPhone,
                                PriceListNum = null,
                                Website = customer.Website,
                                ProjectCode = customer.Codigo_de_Zona_de_Venta__c,
                                UserFields = new List<HitchSapB1Lib.Objects.UserField>
                                {
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_Trato",
                                        Value = (customer.Salutation as string)
                                    },
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_FechadeNac",
                                        Value = customer.PersonBirthdate != null ? Utils.ParseDateTime((customer.PersonBirthdate as string)) : null
                                    },
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_SBO_TIPO_I",
                                        Value = (customer.Tipo_de_institucion__c as string)
                                    },
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_Especialidad",
                                        Value = (customer.Especialidad__c as string)
                                    },
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_subesp1",
                                        Value = (customer.Subespecialidad__c as string)
                                    },
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_CATCLIDERMA",
                                        Value = (customer.Clasificador_ATM_Dermatologia__c as string)
                                    },
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_CATCLIURO",
                                        Value = (customer.Clasificador_ATM_Cirugia__c as string)
                                    },
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_CATCLIOFTA",
                                        Value = (customer.Clasificador_ATM_Oftalmologia__c as string)
                                    },
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_Sexo",
                                        Value = (customer.Genero__c as string)
                                    },
                                    new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_Profesion1",
                                        Value = (customer.Profesion__c as string)
                                    }
                                }
                            };

                            createBusinessPartner.Start();
                        }

                        if (Order.Vendedor.HasValue)
                        {
                            dynamic result = company.QueryOneResult<dynamic>($"SELECT TOP 1 OHEM.empID FROM OHEM WHERE OHEM.salesPrson = {Order.Vendedor.Value}");

                            if (result != null)
                            {
                                SapOrder.OwnerCode = Convert.ToInt32(result.empID);

                                if (SapOrder.OwnerCode == 0)
                                {
                                    SapOrder.OwnerCode = null;
                                }
                            }
                        }


                        if (string.IsNullOrEmpty(Order.ContactSN))
                        {
                            if (Opportunity.Contacto_de_facturacion__c != null)
                            {
                                var createContact = new AddContactToBusinessPartner();
                                createContact.Company = company;
                                createContact.CardCode = Order.CardCode;

                                var contactInfo = salesforceApi.GetSalesForceAction("Contact", Opportunity.Contacto_de_facturacion__c.ToString());

                                if (contactInfo != null && contactInfo?.Id != null)
                                {
                                    createContact.Contact = new Contact
                                    {
                                        Name = contactInfo.Name.ToString(),
                                        FirstName = contactInfo.FirstName == null ? "" : contactInfo.FirstName.ToString(),
                                        LastName = contactInfo.LastName == null ? "" : contactInfo.LastName.ToString(),
                                        Email = contactInfo.Email == null ? "" : contactInfo.Email.ToString(),
                                        Phone1 = contactInfo.Phone == null ? "" : contactInfo.Phone.ToString(),
                                        MobilePhone = contactInfo.MobilePhone == null ? "" : contactInfo.MobilePhone.ToString(),
                                        Title = contactInfo.Title == null ? "" : contactInfo.Title.ToString(),
                                        Active = true
                                    };

                                    createContact.PreExecutionHook = null;
                                    createContact.PostExecutionHook = null;
                                    createContact.Start();

                                    SapOrder.ContactCode = createContact.ContactCode;

                                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                                    {
                                        Name = "U_SBO_CONT",
                                        Value = createContact.ContactName
                                    });
                                }
                                else
                                {
                                    throw new ArgumentException($"No se encontró información del contacto con id {Opportunity.Contacto_de_facturacion__c.ToString()} en Salesforce");
                                }
                            }
                        }
                        else
                        {
                            // Order.ContactSN solo viene valor si en salesforce el contacto fue integrado desde SAP

                            var contactInfo = GetContact(Order.ContactSN);

                            SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                            {
                                Name = "U_SBO_CONT",
                                Value = contactInfo.Name
                            });
                        }


                        dynamic shipAddress = null;


                        if (string.IsNullOrEmpty(Order.ShipToCode))
                        {
                            if (Opportunity.Direccion_de_despacho__c.ToString() != null)
                            {
                                var shipToInfo = salesforceApi.GetSalesForceAction("Direccion_de_despacho__c", Opportunity.Direccion_de_despacho__c.ToString());

                                if (shipToInfo != null && shipToInfo?.Id != null)
                                {
                                    Order.ShipToCode = Utils.Slug($"{Order.CardCode}S{shipToInfo.Name.ToString()}");
                                }
                                else
                                {
                                    throw new ArgumentException($"No se encontró información de la direccion con id {Opportunity.Direccion_de_despacho__c.ToString()} en Salesforce");
                                }
                            }

                        }

                        if (string.IsNullOrEmpty(Order.ShipToCode) == false)
                        {
                            dynamic shipAddressResult = company.QueryOneResult<dynamic>($"SELECT TOP 1 CONCAT(CRD1.CardCode, CRD1.AdresType, CRD1.Address) FROM CRD1 WHERE CRD1.Address = '{Order.ShipToCode}' AND CRD1.CardCode = '{SapOrder.CustomerCode}' AND CRD1.AdresType = 'S'");

                            if (shipAddressResult == null)
                            {
                                dynamic __address = null;

                                if (shipAddress == null)
                                {
                                    __address = salesforceApi.GetDeliveryAddress(Order.ShipToCode) ??
                                                salesforceApi.GetSalesForceAction("Direccion_de_despacho__c", Opportunity.Direccion_de_despacho__c.ToString());
                                }
                                else
                                {
                                    __address = shipAddress;
                                }

                                if (__address == null)
                                {
                                    throw new Exception($"La direccion de despacho codigo {SapOrder.ShipToCode} no ha sido encontrada en SAP y tampoco en Salesforce");
                                }

                                var address = new HitchSapB1Lib.Objects.Definition.Address();

                                string addressId = __address.Id;
                                string addressName = __address.Name;
                                string addressCity = __address.Ciudad__c;
                                string addressStreet = __address.Calle__c;
                                string addressTaxCode = __address.Indicador_de_impuestos__c;

                                address.AddressCode = Utils.Slug($"{Order.CardCode}S{addressName}");
                                address.City = addressCity;
                                address.CountryCode = Order.CountryCode;
                                address.County = Order.CountyCode;
                                address.Name2 = addressName;
                                address.NumGlobalLocation = addressId;
                                address.StateCode = Order.StateCode;
                                address.Street = addressStreet;
                                address.Type = AddressType.Ship;
                                address.TaxCode = addressTaxCode;

                                var createAddress = new AddAddressToBusinessPartner();
                                createAddress.Company = company;
                                createAddress.CardCode = Order.CardCode;
                                createAddress.Address = address;
                                createAddress.PostExecutionHook = null;

                                dynamic __addressWithNewCode = salesforceApi.GetDeliveryAddress(createAddress.Address.AddressCode);

                                if (__addressWithNewCode == null)
                                {
                                    salesforceApi.UpdateDeliveryAddress(createAddress.Address.NumGlobalLocation, createAddress.Address.AddressCode);
                                }

                                createAddress.Start();

                                SapOrder.ShipToCode = createAddress.Address.AddressCode;
                            }
                        }

                        dynamic payAddress = null;

                        if (string.IsNullOrEmpty(Order.PayToCode))
                        {
                            if (Opportunity.Direccion_de_facturacion__c.ToString() != null)
                            {
                                payAddress = salesforceApi.GetSalesForceAction("Direccion_de_despacho__c", Opportunity.Direccion_de_facturacion__c.ToString());

                                if (payAddress != null && payAddress.Id != null)
                                {
                                    Order.PayToCode = Utils.Slug($"{Order.CardCode}B{payAddress.Name.ToString()}");
                                }
                                else
                                {
                                    throw new ArgumentException("No se encontró información de facturación");
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(Order.PayToCode) == false)
                        {
                            dynamic payAddressResult = company.QueryOneResult<dynamic>($"SELECT TOP 1 CONCAT(CRD1.CardCode, CRD1.AdresType, CRD1.Address) FROM CRD1 WHERE CRD1.Address = '{Order.PayToCode}' AND CRD1.CardCode = '{SapOrder.CustomerCode}' AND CRD1.AdresType = 'B'");

                            if (payAddressResult == null)
                            {
                                dynamic __address = null;

                                if (payAddress == null)
                                {
                                    __address = salesforceApi.GetDeliveryAddress(Order.PayToCode) ??
                                                salesforceApi.GetSalesForceAction("Direccion_de_despacho__c", Opportunity.Direccion_de_facturacion__c.ToString());
                                }
                                else
                                {
                                    __address = payAddress;
                                }

                                var address = new HitchSapB1Lib.Objects.Definition.Address();

                                if (__address == null)
                                {
                                    throw new Exception($"La direccion de facturacion codigo {SapOrder.PayToCode} no ha sido encontrada en SAP y tampoco en Salesforce");
                                }

                                string addressId = __address.Id;
                                string addressName = __address.Name;
                                string addressCity = __address.Ciudad__c;
                                string addressStreet = __address.Calle__c;
                                string addressTaxCode = __address.Indicador_de_impuestos__c;

                                address.AddressCode = Utils.Slug($"{Order.CardCode}B{addressName}");
                                address.City = addressCity;
                                address.CountryCode = Order.CountryCode;
                                address.County = Order.CountyCode;
                                address.Name2 = addressName;
                                address.NumGlobalLocation = addressId;
                                address.StateCode = Order.StateCode;
                                address.Street = addressStreet;
                                address.Type = AddressType.Pay;
                                address.TaxCode = addressTaxCode;

                                var createAddress = new AddAddressToBusinessPartner();
                                createAddress.Company = company;
                                createAddress.CardCode = Order.CardCode;
                                createAddress.Address = address;
                                createAddress.PreExecutionHook = null;
                                createAddress.PostExecutionHook = null;

                                dynamic __addressWithNewCode = salesforceApi.GetDeliveryAddress(createAddress.Address.AddressCode);

                                if (__addressWithNewCode == null)
                                {
                                    salesforceApi.UpdateDeliveryAddress(createAddress.Address.NumGlobalLocation, createAddress.Address.AddressCode);
                                }

                                createAddress.Start();

                                SapOrder.PayToCode = createAddress.Address.AddressCode;
                            }
                        }

                        Utils.CleanAddresses(Order.CardCode, company);

                        return new HookResult
                        {
                            Exception = null,
                            Result = SapOrder
                        };
                    }
                    catch (Exception ex)
                    {
                        return new HookResult
                        {
                            Exception = ex,
                            Result = null
                        };
                    }
                };

                createSaleOrderOperation.PostExecutionHook = null;
                createSaleOrderOperation.Company = company;

                createSaleOrderOperation.Start();
                var responseSap = new SapResponse
                {
                    DocEntry = createSaleOrderOperation.DocEntry.Value,
                    DocNum = createSaleOrderOperation.DocNum.Value,
                    CustomerCodeSap = SapOrder.CustomerCode,
                    CustomerCodeSalesforce = Opportunity.AccountId.ToString(),
                    ContactCodeSap = SapOrder.ContactCode.Value,
                    ContactCodeSalesforce = Opportunity.Contacto_de_facturacion__c?.ToString(),
                    ShipToCodeSap = SapOrder.ShipToCode,
                    ShipToCodeSalesforce = Opportunity.Direccion_de_despacho__c?.ToString(),
                    PayToCodeSap = SapOrder.PayToCode,
                    PayToCodeSalesforce = Opportunity.Direccion_de_facturacion__c?.ToString()
                };
                return responseSap;
            }
        }


        public Tuple<int, int> CreatePurchaseOrder(HitchAtmApi.Models.PurchaseOrder Order)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                CreatePurchaseOrderOperation createPurchaseOrderOperation = new CreatePurchaseOrderOperation();

                HitchSapB1Lib.Objects.Shopping.PurchaseOrder SapOrder = new HitchSapB1Lib.Objects.Shopping.PurchaseOrder
                {
                    ProviderCode = Order.CardCode,
                    DocumentDate = Order.DocDate.Value,
                    DueDate = Order.DocDueDate.Value,
                    TaxDate = Order.TaxDate.Value,
                    Discount = null,
                    PayToCode = null,
                    ShipToCode = null,
                    CurrencySource = CurrencySource.Customer,
                    Comment = Order.Comments,
                    OwnerCode = null,
                    ProviderReferenceNumber = null,
                    BuyEmployeeCode = Order.SlpCode,
                    ContactCode = null,
                    Serie = Order.Serie,
                    UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                    Lines = Order.Detail.Select(det =>
                    {
                        var line = new DocumentLine
                        {
                            ItemCode = det.ItemCode,
                            TaxCode = "IVA_EXE",
                            Quantity = det.Quantity,
                            Discount = null,
                            Description = null,
                            Price = det.Importe,
                            DeliveryDate = null,
                            Batchs = null,
                            CurrencyCode = null,
                            UserFields = null,
                            Reference = null,
                            Series = null,
                            CostingCode1 = det.Dim1,
                            CostingCode2 = det.Dim2,
                            CostingCode3 = det.Dim3,
                            Project = det.Zona
                        };

                        return line;
                    }).ToList(),
                };

                if (string.IsNullOrEmpty(Order.Doc) == false)
                {
                    string fileLocation = Utils.SavePDFAttachment(
                        System.Guid.NewGuid().ToString() +
                        (string.IsNullOrEmpty(Order.DocExt) ? "" : ("." + Order.DocExt.Replace(".", ""))), Order.Doc);

                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOCC",
                        Value = fileLocation
                    });
                }

                if (string.IsNullOrEmpty(Order.TipoSolic) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBO_TIPOSR",
                        Value = Order.TipoSolic
                    });
                }

                if (Order.FechaSalida.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFECHSAL",
                        Value = Order.FechaSalida.Value
                    });
                }

                if (Order.FechaLlegada.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFECHLLEG",
                        Value = Order.FechaLlegada.Value
                    });
                }

                if (Order.NumVisitas.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBONUMVIS",
                        Value = Order.NumVisitas.Value
                    });
                }

                if (string.IsNullOrEmpty(Order.OportVentas) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBODETON",
                        Value = Order.OportVentas
                    });
                }

                if (string.IsNullOrEmpty(Order.ObsCobertura) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOBCOB",
                        Value = Order.ObsCobertura
                    });
                }

                if (string.IsNullOrEmpty(Order.HoraSalidaStgo) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOHORSAL",
                        Value = Order.HoraSalidaStgo
                    });
                }

                if (string.IsNullOrEmpty(Order.HoraUltimaVisita) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOHORLLEG",
                        Value = Order.HoraUltimaVisita
                    });
                }

                if (string.IsNullOrEmpty(Order.CiudadesDest) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCIUDES",
                        Value = Order.CiudadesDest
                    });
                }

                if (Order.OCOrigen.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOCORIGEN",
                        Value = Order.OCOrigen.Value
                    });
                }

                if (string.IsNullOrEmpty(Order.Discount) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_DsctItmT",
                        Value = Order.Discount
                    });
                }

                createPurchaseOrderOperation.PurchaseOrder = SapOrder;
                createPurchaseOrderOperation.PostExecutionHook = null;
                createPurchaseOrderOperation.Company = company;
                createPurchaseOrderOperation.Start();

                return new Tuple<int, int>(createPurchaseOrderOperation.DocEntry.Value,
                    createPurchaseOrderOperation.DocNum.Value);
            }
        }

        public void UpdatePurchaseOrder(int DocEntry, HitchAtmApi.Models.PurchaseOrder Order)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                UpdatePurchaseOrderOperation updatePurchaseOrderOperation = new UpdatePurchaseOrderOperation();

                HitchSapB1Lib.Objects.Shopping.PurchaseOrder SapOrder = new HitchSapB1Lib.Objects.Shopping.PurchaseOrder
                {
                    ProviderCode = Order.CardCode,
                    DocumentDate = Order.DocDate.Value,
                    DueDate = Order.DocDueDate.Value,
                    TaxDate = Order.TaxDate.Value,
                    Discount = null,
                    PayToCode = null,
                    ShipToCode = null,
                    CurrencySource = CurrencySource.Customer,
                    Comment = Order.Comments,
                    OwnerCode = null,
                    ProviderReferenceNumber = null,
                    BuyEmployeeCode = Order.SlpCode,
                    ContactCode = null,
                    Serie = Order.Serie,
                    UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                    Lines = Order.Detail.Select(det =>
                    {
                        var line = new DocumentLine
                        {
                            ItemCode = det.ItemCode,
                            TaxCode = "IVA_EXE",
                            Quantity = det.Quantity,
                            Discount = null,
                            Description = null,
                            Price = det.Importe,
                            DeliveryDate = null,
                            Batchs = null,
                            CurrencyCode = null,
                            UserFields = null,
                            Reference = null,
                            Series = null,
                            CostingCode1 = det.Dim1,
                            CostingCode2 = det.Dim2,
                            CostingCode3 = det.Dim3,
                            Project = det.Zona
                        };

                        return line;
                    }).ToList(),
                };

                if (string.IsNullOrEmpty(Order.Doc) == false)
                {
                    string fileLocation = Utils.SavePDFAttachment(
                        System.Guid.NewGuid().ToString() +
                        (string.IsNullOrEmpty(Order.DocExt) ? "" : ("." + Order.DocExt.Replace(".", ""))), Order.Doc);

                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOCC",
                        Value = fileLocation
                    });
                }

                if (string.IsNullOrEmpty(Order.TipoSolic) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBO_TIPOSR",
                        Value = Order.TipoSolic
                    });
                }

                if (Order.FechaSalida.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFECHSAL",
                        Value = Order.FechaSalida.Value
                    });
                }

                if (Order.FechaLlegada.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFECHLLEG",
                        Value = Order.FechaLlegada.Value
                    });
                }

                if (Order.NumVisitas.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBONUMVIS",
                        Value = Order.NumVisitas.Value
                    });
                }

                if (string.IsNullOrEmpty(Order.OportVentas) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBODETON",
                        Value = Order.OportVentas
                    });
                }

                if (string.IsNullOrEmpty(Order.ObsCobertura) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOBCOB",
                        Value = Order.ObsCobertura
                    });
                }

                if (string.IsNullOrEmpty(Order.HoraSalidaStgo) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOHORSAL",
                        Value = Order.HoraSalidaStgo
                    });
                }

                if (string.IsNullOrEmpty(Order.HoraUltimaVisita) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOHORLLEG",
                        Value = Order.HoraUltimaVisita
                    });
                }

                if (string.IsNullOrEmpty(Order.CiudadesDest) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCIUDES",
                        Value = Order.CiudadesDest
                    });
                }

                if (Order.OCOrigen.HasValue)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOCORIGEN",
                        Value = Order.OCOrigen.Value
                    });
                }

                if (string.IsNullOrEmpty(Order.Discount) == false)
                {
                    SapOrder.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_DsctItmT",
                        Value = Order.Discount
                    });
                }

                updatePurchaseOrderOperation.PurchaseOrder = SapOrder;
                updatePurchaseOrderOperation.DocEntry = DocEntry;
                updatePurchaseOrderOperation.PostExecutionHook = null;
                updatePurchaseOrderOperation.Company = company;
                updatePurchaseOrderOperation.Start();
            }
        }

        public Tuple<int, int> CreateTransferRequest(HitchAtmApi.Models.TransferRequest Transfer)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                CreateTransferRequestOperation createTransferRequestOperation = new CreateTransferRequestOperation();

                HitchSapB1Lib.Objects.Inventory.TransferRequest SapTransferRequest = new HitchSapB1Lib.Objects.Inventory.TransferRequest
                {
                    CustomerCode = Transfer.CardCode,
                    ContactCode = Transfer.Contacto,
                    Comment = Transfer.Comments,
                    DocumentDate = Transfer.DocDate.Value,
                    DueDate = Transfer.DocDueDate.Value,
                    TaxDate = Transfer.TaxDate.Value,
                    StartDeliveryDate = null,
                    EndDeliveryDate = null,
                    FromWarehouseCode = Transfer.AlmacenOrigen,
                    ToWarehouseCode = Transfer.AlmacenDestino,
                    JournalMemo = Transfer.ImlMeno,
                    PriceListCode = null,
                    SalesEmployeeCode = Transfer.SlpCode.HasValue ? Convert.ToInt32(Transfer.SlpCode) : (int?)null,
                    Serie = null,
                    ShipToCode = null,
                    UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                    Attachments = new List<string>(),
                    Lines = Transfer.Detail.Select(det => new TransferRequestLine
                    {
                        ItemCode = det.ItemCode,
                        Quantity = det.Quantity,
                        FromWarehouseCode = det.Almacen,
                        ToWarehouseCode = det.AlmacenDest,
                        Reference = null,
                        Batchs = null,
                        Series = null,
                        CostingCode1 = det.Dim1,
                        CostingCode2 = det.Dim2,
                        CostingCode3 = det.Dim3,
                        Project = det.Zona,
                        UserFields = null
                    }).ToList()
                };

                if (string.IsNullOrEmpty(Transfer.TipoPD) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOTIPOPD",
                        Value = Transfer.TipoPD
                    });
                }

                if (string.IsNullOrEmpty(Transfer.TipoPD) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOTIPOPD",
                        Value = Transfer.TipoPD
                    });
                }

                if (string.IsNullOrEmpty(Transfer.CodeSN) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCODCLIPD",
                        Value = Transfer.TipoPD
                    });
                }

                if (string.IsNullOrEmpty(Transfer.RazonSocial) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBONOMCLIPD",
                        Value = Transfer.RazonSocial
                    });
                }

                if (Transfer.DateStart.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFINIPD",
                        Value = Transfer.DateStart.Value
                    });
                }

                if (Transfer.DateEnd.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFTERPD",
                        Value = Transfer.DateEnd.Value
                    });
                }

                if (string.IsNullOrEmpty(Transfer.Ubicacion) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOUBIPD",
                        Value = Transfer.Ubicacion
                    });
                }

                if (string.IsNullOrEmpty(Transfer.Observaciones) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOBSPD",
                        Value = Transfer.Observaciones
                    });
                }

                if (Transfer.NumOVSF.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOPVE",
                        Value = Transfer.NumOVSF.Value
                    });
                }

                if (Transfer.LlamadaServ.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOLLSER",
                        Value = Transfer.LlamadaServ.Value
                    });
                }

                if (string.IsNullOrEmpty(Transfer.Anexo) == false)
                {
                    string fileLocation = Utils.SavePDFAttachment(
                        (Guid.NewGuid().ToString() + (string.IsNullOrEmpty(Transfer.AnexoExt) ? "" : ("." + Transfer.AnexoExt.Replace(".", "")))), Transfer.Anexo);

                    SapTransferRequest.Attachments.Add(fileLocation);
                }

                createTransferRequestOperation.TransferRequest = SapTransferRequest;
                createTransferRequestOperation.PostExecutionHook = null;
                createTransferRequestOperation.Company = company;
                createTransferRequestOperation.Start();

                return new Tuple<int, int>(createTransferRequestOperation.DocEntry.Value,
                    createTransferRequestOperation.DocNum.Value);
            }
        }

        public void UpdateTransferRequest(int DocEntry, HitchAtmApi.Models.TransferRequest Transfer)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                UpdateTransferRequestOperation updateTransferRequestOperation = new UpdateTransferRequestOperation();

                HitchSapB1Lib.Objects.Inventory.TransferRequest SapTransferRequest = new HitchSapB1Lib.Objects.Inventory.TransferRequest
                {
                    CustomerCode = Transfer.CardCode,
                    ContactCode = Transfer.Contacto,
                    Comment = Transfer.Comments,
                    DocumentDate = Transfer.DocDate.Value,
                    DueDate = Transfer.DocDueDate.Value,
                    TaxDate = Transfer.TaxDate.Value,
                    StartDeliveryDate = null,
                    EndDeliveryDate = null,
                    FromWarehouseCode = Transfer.AlmacenOrigen,
                    ToWarehouseCode = Transfer.AlmacenDestino,
                    JournalMemo = Transfer.ImlMeno,
                    PriceListCode = null,
                    SalesEmployeeCode = Transfer.SlpCode.HasValue ? Convert.ToInt32(Transfer.SlpCode) : (int?)null,
                    Serie = null,
                    ShipToCode = null,
                    UserFields = new List<HitchSapB1Lib.Objects.UserField>(),
                    Attachments = new List<string>(),
                    Lines = Transfer.Detail.Select(det => new TransferRequestLine
                    {
                        ItemCode = det.ItemCode,
                        Quantity = det.Quantity,
                        FromWarehouseCode = det.Almacen,
                        ToWarehouseCode = det.AlmacenDest,
                        Reference = null,
                        Batchs = null,
                        Series = null,
                        CostingCode1 = det.Dim1,
                        CostingCode2 = det.Dim2,
                        CostingCode3 = det.Dim3,
                        Project = det.Zona,
                        UserFields = null
                    }).ToList()
                };

                if (string.IsNullOrEmpty(Transfer.TipoPD) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOTIPOPD",
                        Value = Transfer.TipoPD
                    });
                }

                if (string.IsNullOrEmpty(Transfer.TipoPD) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOTIPOPD",
                        Value = Transfer.TipoPD
                    });
                }

                if (string.IsNullOrEmpty(Transfer.CodeSN) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOCODCLIPD",
                        Value = Transfer.TipoPD
                    });
                }

                if (string.IsNullOrEmpty(Transfer.RazonSocial) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBONOMCLIPD",
                        Value = Transfer.RazonSocial
                    });
                }

                if (Transfer.DateStart.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFINIPD",
                        Value = Transfer.DateStart.Value
                    });
                }

                if (Transfer.DateEnd.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOFTERPD",
                        Value = Transfer.DateEnd.Value
                    });
                }

                if (string.IsNullOrEmpty(Transfer.Ubicacion) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOUBIPD",
                        Value = Transfer.Ubicacion
                    });
                }

                if (string.IsNullOrEmpty(Transfer.Observaciones) == false)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOBSPD",
                        Value = Transfer.Observaciones
                    });
                }

                if (Transfer.NumOVSF.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOOPVE",
                        Value = Transfer.NumOVSF.Value
                    });
                }

                if (Transfer.LlamadaServ.HasValue)
                {
                    SapTransferRequest.UserFields.Add(new HitchSapB1Lib.Objects.UserField
                    {
                        Name = "U_SBOLLSER",
                        Value = Transfer.LlamadaServ.Value
                    });
                }

                if (string.IsNullOrEmpty(Transfer.Anexo) == false)
                {
                    string fileLocation = Utils.SavePDFAttachment(
                        (Guid.NewGuid().ToString() + (string.IsNullOrEmpty(Transfer.AnexoExt) ? "" : ("." + Transfer.AnexoExt.Replace(".", "")))), Transfer.Anexo);

                    SapTransferRequest.Attachments.Add(fileLocation);
                }

                updateTransferRequestOperation.TransferRequest = SapTransferRequest;
                updateTransferRequestOperation.DocEntry = DocEntry;
                updateTransferRequestOperation.PostExecutionHook = null;
                updateTransferRequestOperation.Company = company;
                updateTransferRequestOperation.Start();
            }
        }

        public Customer GetCustomer(string LicTradeNum)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                return company.QueryOneResult<Customer>(string.Format(
                    @"SELECT T0.CardCode, T0.CardName, T0.CardFName, T0.LicTradNum FROM OCRD T0
                    WHERE T0.LicTradNum = '{0}' AND T0.CardType = 'C'", LicTradeNum));
            }
        }

        public ContactIdentification GetContact(string contactCode)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                return company.QueryOneResult<ContactIdentification>(string.Format(
                     "SELECT T0.CntctCode, T0.Name FROM OCPR T0 WHERE T0.CntctCode = {0}", contactCode));
            }
        }

        public Product GetProduct(string ItemCode)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                return company.QueryOneResult<Product>(string.Format(
                    @"SELECT T0.ItemCode, T0.ItemName, (CASE T0.frozenFor WHEN 'Y' THEN 0 ELSE 1 END) AS ""StatusProduct"" FROM OITM T0
                    WHERE T0.ItemCode = '{0}'", ItemCode));
            }
        }

        public void UpdateCard(int InsID, string Status, HitchAtmApi.Models.Address Address)
        {
            using (Company company = new Company(DefaultConnectionParameters))
            {
                UpdateEquipmentCardOperation updateEquipmentCardOperation = new UpdateEquipmentCardOperation();

                EquipmentCardStatus CardStatus = EquipmentCardStatus.Active;

                if (Status.Equals("A"))
                {
                    CardStatus = EquipmentCardStatus.Active;
                }
                else if (Status.Equals("I"))
                {
                    CardStatus = EquipmentCardStatus.InLab;
                }
                else if (Status.Equals("L"))
                {
                    CardStatus = EquipmentCardStatus.Loaned;
                }
                else if (Status.Equals("R"))
                {
                    CardStatus = EquipmentCardStatus.Returned;
                }
                else if (Status.Equals("T"))
                {
                    CardStatus = EquipmentCardStatus.Terminated;
                }
                else
                {
                    throw new Exception(
                        "El valor asignado al estado de la tarjeta de equipo no es valido. Valores validos (A: Activo, I: En laboratorio, L: en prestamo, R: Devuelto, T: Terminado");
                }

                HitchSapB1Lib.Objects.Services.EquipmentCard SapCardEquipment = new HitchSapB1Lib.Objects.Services.EquipmentCard
                {
                    CardStatus = CardStatus,
                    Address = Address != null
                     ? new HitchSapB1Lib.Objects.Definition.Address
                     {
                         Block = Address.Block,
                         Building = Address.Building,
                         City = Address.City,
                         Street = Address.Street,
                         StateCode = Address.StateCode,
                         CountryCode = Address.CountryCode,
                         County = Address.County,
                         StreetNumber = Address.StreetNumber,
                         Zip = Address.Zip
                     }
                     : null
                };

                updateEquipmentCardOperation.UpdateEquipmentCardParams = new UpdateEquipmentCardParams
                {
                    CardId = InsID,
                    EquipmentCard = SapCardEquipment
                };
                updateEquipmentCardOperation.Company = company;
                updateEquipmentCardOperation.Start();
            }
        }
    }
}
