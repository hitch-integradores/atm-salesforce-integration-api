using System.Collections.Generic;
using System.Threading.Tasks;
using HitchAtmApi.Models;

namespace HitchAtmApi.Lib
{
    public class SalesOrdersService
    {
        ConnectionParameters DefaultConnectionParameters;

        public SalesOrdersService(ConnectionParameters connectionParameters)
        {
            DefaultConnectionParameters = connectionParameters;
        }

        async public Task<List<SaleOrderDetail>> GetSaleOrderDetail(long orderId)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryAll<SaleOrderDetail>(
                    "SELECT * FROM SalesOrdersDetail WHERE OrderId = @OrderId",
                    new
                    {
                        OrderId= orderId
                    });
            }
        }

        async public Task<SaleOrderDetail> GetSaleOrderLine(long lineId)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryOne<SaleOrderDetail>(
                    "SELECT * FROM SalesOrdersDetail WHERE Id = @Id",
                    new
                    {
                        Id = lineId
                    });
            }
        }

        async public Task<SaleOrder> GetSaleOrder(string CodSF)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryOne<SaleOrder>(
                    "SELECT * FROM SalesOrders WHERE CodSF = @Code",
                    new
                    {
                        Code = CodSF
                    });
            }
        }

        async public Task<long> SaveSaleOrder(SaleOrder order)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"INSERT INTO SalesOrders (CardCode,CNTCCode,DocDate,DocDueDate,TaxDate,Descuento,Vendedor,ShipToCode,PayToCode,PartSuply,Project,CapacitacionReq,GarantiaPactada,MantPreventivo,NumVisitasAnoGarantia,OCcliente,DocOCCliente,ExistenMultas,DateOCCLiente,DateRecepcionOC,ContactSN,RutSN,NomSN,NumCotizacionProv,CancFaltaStock,LeasingATM,DirecEntregaFactura,CodSF)
                    VALUES (@CardCode,@CNTCCode,@DocDate,@DocDueDate,@TaxDate,@Descuento,@Vendedor,@ShipToCode,@PayToCode,@PartSuply,@Project,@CapacitacionReq,@GarantiaPactada,@MantPreventivo,@NumVisitasAnoGarantia,@OCcliente,@DocOCCliente,@ExistenMultas,@DateOCCLiente,@DateRecepcionOC,@ContactSN,@RutSN,@NomSN,@NumCotizacionProv,@CancFaltaStock,@LeasingATM,@DirecEntregaFactura,@CodSF)",
                    order);

                return await provider.QueryOne<long>("SELECT MAX(Id) FROM SalesOrders");
            }
        }

        async public Task UpdateSaleOrder(SaleOrder order)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"UPDATE SalesOrders SET CardCode = @CardCode, CNTCCode = @CNTCCode, DocDate = @DocDate, DocDueDate = @DocDueDate,
                    TaxDate = @TaxDate, Descuento = @Descuento, Vendedor = @Vendedor, ShipToCode = @ShipToCode, PayToCode = @PayToCode,
                    PartSuply = @PartSuply, Project = @Project, CapacitacionReq = @CapacitacionReq, GarantiaPactada = @GarantiaPactada,
                    MantPreventivo = @MantPreventivo, NumVisitasAnoGarantia = @NumVisitasAnoGarantia, OCcliente = @OCcliente,
                    DocOCCliente = @DocOCCliente, ExistenMultas = @ExistenMultas, DateOCCLiente = @DateOCCLiente, DateRecepcionOC = @DateRecepcionOC,
                    ContactSN = @ContactSN, RutSN = @RutSN, NomSN = @NomSN, NumCotizacionProv = @NumCotizacionProv, CancFaltaStock = @CancFaltaStock,
                    LeasingATM = @LeasingATM, DirecEntregaFactura = @DirecEntregaFactura WHERE Id = @Id",
                    order);
            }
        }

        async public Task<long> SaveSaleOrderLine(SaleOrderDetail line)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"INSERT INTO SalesOrdersDetail (ItemCode,Quantity,Descuento,UnitPrice,Almacen,DateEntrega,Vendedor,Comments,Description,IDSF,OrderId)
                    VALUES (@ItemCode,@Quantity,@Descuento,@UnitPrice,@Almacen,@DateEntrega,@Vendedor,@Comments,@Description,@IDSF,@OrderId)",
                    line);

                return await provider.QueryOne<long>("SELECT MAX(Id) FROM SalesOrdersDetail");
            }
        }

        async public Task UpdateSaleOrderLine(SaleOrderDetail line)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"UPDATE SalesOrdersDetail SET ItemCode = @ItemCode, Quantity = @Quantity, Descuento = @Descuento, UnitPrice = @UnitPrice,
                    Almacen = @Almacen, DateEntrega = @DateEntrega, Vendedor = @Vendedor, Comments = @Comments, Description = @Description,
                    IDSF = @IDSF WHERE Id = @Id",
                    line);
            }
        }

        async public Task DeleteOrder(long Id)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    "DELETE FROM SalesOrders WHERE Id = @Id;DELETE FROM SalesOrdersDetail WHERE OrderId = @Id",
                    new
                    {
                        Id
                    });
            }
        }
    }
}
