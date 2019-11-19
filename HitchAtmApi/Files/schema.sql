CREATE SEQUENCE sales_orders_id_seq;

CREATE TABLE SalesOrders (
    Id BIGINT NOT NULL DEFAULT nextval('sales_orders_id_seq'),
    CardCode VARCHAR(50) NULL,
    CNTCCode INTEGER NULL,
    DocDate TIMESTAMP NULL,
    DocDueDate TIMESTAMP NULL,
    TaxDate TIMESTAMP NULL,
    Descuento VARCHAR(50) NULL,
    Vendedor INTEGER NULL,
    ShipToCode VARCHAR(100) NULL,
    PayToCode VARCHAR(20) NULL,
    PartSuply BOOLEAN NULL,
    Project VARCHAR(100) NULL,
    CapacitacionReq VARCHAR(100) NULL,
    GarantiaPactada VARCHAR(200) NULL,
    MantPreventivo VARCHAR(200) NULL,
    NumVisitasAnoGarantia INTEGER NULL,
    OCcliente VARCHAR(200) NULL,
    DocOCCliente VARCHAR(200) NULL,
    ExistenMultas VARCHAR(200) NULL,
    DateOCCLiente TIMESTAMP NULL,
    DateRecepcionOC TIMESTAMP NULL,
    ContactSN VARCHAR(200) NULL,
    RutSN VARCHAR(200) NULL,
    NomSN VARCHAR(200) NULL,
    NumCotizacionProv VARCHAR(200) NULL,
    CancFaltaStock BOOLEAN NULL,
    LeasingATM BOOLEAN NULL,
    DirecEntregaFactura VARCHAR(200) NULL,
    CodSF VARCHAR(200) NULL,
    PRIMARY KEY (Id)
);

ALTER SEQUENCE sales_orders_id_seq
OWNED BY SalesOrders.Id;

CREATE SEQUENCE sales_orders_detail_id_seq;

CREATE TABLE SalesOrdersDetail (
    Id BIGINT NOT NULL DEFAULT nextval('sales_orders_detail_id_seq'),
    ItemCode VARCHAR(100) NULL,
    Quantity INTEGER NULL,
    Descuento VARCHAR(50) NULL,
    UnitPrice DECIMAL NULL,
    Almacen VARCHAR(100) NULL,
    DateEntrega TIMESTAMP NULL,
    Vendedor INTEGER NULL,
    Comments VARCHAR(200) NULL,
    Description VARCHAR(200) NULL,
    IDSF VARCHAR(200) NULL,
    OrderId BIGINT NULL,
    PRIMARY KEY (Id)
);

ALTER SEQUENCE sales_orders_detail_id_seq
OWNED BY SalesOrdersDetail.Id;

CREATE SEQUENCE transfers_requests_id_seq;

CREATE TABLE TransfersRequests (
    Id BIGINT NOT NULL DEFAULT nextval('transfers_requests_id_seq'),
    CardCode VARCHAR(50) NULL,
    Contacto INTEGER NULL,
    DocDate TIMESTAMP NULL,
    DocDueDate TIMESTAMP NULL,
    TaxDate TIMESTAMP NULL,
    AlmacenOrigen VARCHAR(100) NULL,
    AlmacenDestino VARCHAR(100) NULL,
    SlpCode INTEGER NULL,
    ImlMeno VARCHAR(200) NULL,
    TipoPD VARCHAR(100) NULL,
    CodeSN VARCHAR(200) NULL,
    RazonSocial VARCHAR(200) NULL,
    DateStart TIMESTAMP NULL,
    DateEnd TIMESTAMP NULL,
    Ubicacion VARCHAR(200) NULL,
    Observaciones VARCHAR(200) NULL,
    NumOVSF INTEGER NULL,
    LlamadaServ INTEGER NULL,
    Anexo VARCHAR(200) NULL,
	CodSF VARCHAR(200) NULL,
    PRIMARY KEY (Id)
);

ALTER SEQUENCE transfers_requests_id_seq
OWNED BY TransfersRequests.Id;

CREATE SEQUENCE transfers_requests_detail_id_seq;

CREATE TABLE TransfersRequestsDetail (
    Id BIGINT NOT NULL DEFAULT nextval('transfers_requests_detail_id_seq'),
    ItemCode VARCHAR(100) NULL,
    Almacen VARCHAR(100) NULL,
    AlmacenDest VARCHAR(100) NULL,
    Quantity INTEGER NULL,
    TransferId BIGINT NULL,
    PRIMARY KEY (Id)
);

ALTER SEQUENCE transfers_requests_detail_id_seq
OWNED BY TransfersRequestsDetail.Id;

CREATE SEQUENCE purchases_orders_id_seq;

CREATE TABLE PurchasesOrders (
    Id BIGINT NOT NULL DEFAULT nextval('purchases_orders_id_seq'),
    CardCode VARCHAR(100) NULL,
    DocDate TIMESTAMP NULL,
    DocDueDate TIMESTAMP NULL,
    TaxDate TIMESTAMP NULL,
    Serie INTEGER NULL,
    TipoSolic VARCHAR(100) NULL,
    FechaSalida TIMESTAMP NULL,
    FechaLlegada TIMESTAMP NULL,
    NumVisitas INTEGER NULL,
    OportVentas VARCHAR(100) NULL,
    ObsCobertura VARCHAR(200) NULL,
    HoraSalidaStgo VARCHAR(100) NULL,
    HoraUltimaVisita VARCHAR(100) NULL,
    CiudadesDest VARCHAR(200) NULL,
    OCOrigen INTEGER NULL,
    Discount VARCHAR(50) NULL,
    CodSF VARCHAR(100) NULL,
    PRIMARY KEY (Id)
);

ALTER SEQUENCE purchases_orders_id_seq
OWNED BY PurchasesOrders.Id;

CREATE SEQUENCE purchases_orders_detail_id_seq;

CREATE TABLE PurchasesOrdersDetail (
    Id BIGINT NOT NULL DEFAULT nextval('purchases_orders_detail_id_seq'),
    ItemCode VARCHAR(100) NULL,
    Quantity INTEGER NULL,
    Dim1 VARCHAR(100) NULL,
    Dim2 VARCHAR(100) NULL,
    Dim3 VARCHAR(100) NULL,
    OrderId BIGINT NULL,
    PRIMARY KEY (Id)
);

ALTER SEQUENCE purchases_orders_detail_id_seq
OWNED BY PurchasesOrdersDetail.Id;

CREATE SEQUENCE notifications_id_seq;

CREATE TABLE Notifications (
    Id BIGINT NOT NULL DEFAULT nextval('notifications_id_seq'),
    RefId BIGINT NOT NULL,
    RefType VARCHAR(100) NOT NULL,
    Status VARCHAR(20) NOT NULL,
    CreateTime TIMESTAMP NOT NULL,
    FinishTime TIMESTAMP NULL,
    Steps INTEGER NOT NULL,
    Stage VARCHAR(100) NOT NULL,
    DocNum VARCHAR(100) NULL,
    DocEntry VARCHAR(100) NULL,
    PRIMARY KEY (Id)
);

ALTER SEQUENCE notifications_id_seq
OWNED BY Notifications.Id;

CREATE SEQUENCE api_logs_id_seq;

CREATE TABLE ApiLogs (
    Id BIGINT NOT NULL DEFAULT nextval('api_logs_id_seq'),
    RequestTime TIMESTAMP NOT NULL,
    ResponseMillis BIGINT NOT NULL,
    StatusCode INTEGER NOT NULL,
    Method VARCHAR(100) NOT NULL,
    Path VARCHAR(200) NOT NULL,
    QueryString VARCHAR(200) NULL,
    RequestBody TEXT NULL,
    ResponseBody TEXT NULL,
    PRIMARY KEY (Id)
);

ALTER SEQUENCE api_logs_id_seq
OWNED BY ApiLogs.Id;

CREATE SEQUENCE notifications_logs_id_seq;

CREATE TABLE NotificationsLogs (
    Id BIGINT NOT NULL DEFAULT nextval('notifications_logs_id_seq'),
    CreateTime TIMESTAMP NOT NULL,
    Status VARCHAR(100) NOT NULL,
    Operation VARCHAR(200) NOT NULL,
    Message TEXT NULL,
    NotificationId BIGINT NOT NULL,
    PRIMARY KEY (Id)
);

ALTER SEQUENCE notifications_logs_id_seq
OWNED BY NotificationsLogs.Id;