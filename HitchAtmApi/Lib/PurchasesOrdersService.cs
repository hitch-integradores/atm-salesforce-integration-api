using System.Collections.Generic;
using System.Threading.Tasks;
using HitchAtmApi.Models;

namespace HitchAtmApi.Lib
{
    public class PurchasesOrdersService
    {
        ConnectionParameters DefaultConnectionParameters;

        public PurchasesOrdersService(ConnectionParameters connectionParameters)
        {
            DefaultConnectionParameters = connectionParameters;
        }

        async public Task<List<PurchaseOrderDetail>> GetPurchaseOrderDetail(long orderId)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryAll<PurchaseOrderDetail>(
                    "SELECT * FROM PurchasesOrdersDetail WHERE OrderId = @OrderId",
                    new
                    {
                        OrderId = orderId
                    });
            }
        }

        async public Task<PurchaseOrderDetail> GetPurchaseOrderLine(long lineId)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryOne<PurchaseOrderDetail>(
                    "SELECT * FROM PurchasesOrdersDetail WHERE Id = @Id",
                    new
                    {
                        Id = lineId
                    });
            }
        }

        async public Task<PurchaseOrder> GetPurchaseOrder(string CodSF)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryOne<PurchaseOrder>(
                    "SELECT * FROM PurchasesOrders WHERE CodSF = @Code",
                    new
                    {
                        Code = CodSF
                    });
            }
        }

        async public Task<long> SavePurchaseOrder(PurchaseOrder order)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"INSERT INTO PurchasesOrders (CardCode,DocDate,DocDueDate,TaxDate,Serie,TipoSolic,FechaSalida,FechaLlegada,NumVisitas,OportVentas,ObsCobertura,HoraSalidaStgo,HoraUltimaVisita,CiudadesDest,OCOrigen,Discount,CodSF)
                    VALUES (@CardCode,@DocDate,@DocDueDate,@TaxDate,@Serie,@TipoSolic,@FechaSalida,@FechaLlegada,@NumVisitas,@OportVentas,@ObsCobertura,@HoraSalidaStgo,@HoraUltimaVisita,@CiudadesDest,@OCOrigen,@Discount,@CodSF)",
                    order);

                return await provider.QueryOne<long>("SELECT MAX(Id) FROM PurchasesOrders");
            }
        }

        async public Task<long> SavePurchaseOrderLine(PurchaseOrderDetail line)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"INSERT INTO PurchasesOrdersDetail (ItemCode,Quantity,Dim1,Dim2,Dim3,OrderId)
                    VALUES (@ItemCode,@Quantity,@Dim1,@Dim2,@Dim3,@OrderId)",
                    line);

                return await provider.QueryOne<long>("SELECT MAX(Id) FROM PurchasesOrdersDetail");
            }
        }

        async public Task DeleteOrder(long Id)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    "DELETE FROM PurchasesOrders WHERE Id = @Id;DELETE FROM PurchasesOrdersDetail WHERE OrderId = @Id",
                    new
                    {
                        Id
                    });
            }
        }
    }
}
