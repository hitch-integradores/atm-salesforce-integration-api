using System.Collections.Generic;
using System.Threading.Tasks;
using HitchAtmApi.Models;

namespace HitchAtmApi.Lib
{
    public class TransfersRequestsService
    {
        ConnectionParameters DefaultConnectionParameters;

        public TransfersRequestsService(ConnectionParameters connectionParameters)
        {
            DefaultConnectionParameters = connectionParameters;
        }

        async public Task<List<TransferRequestDetail>> GetTransferRequestDetail(long transferId)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryAll<TransferRequestDetail>(
                    "SELECT * FROM TransfersRequestsDetail WHERE TransferId = @TransferId",
                    new
                    {
                        TransferId = transferId
                    });
            }
        }

        async public Task<TransferRequestDetail> GetTransferRequestLine(long lineId)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryOne<TransferRequestDetail>(
                    "SELECT * FROM TransfersRequestsDetail WHERE Id = @Id",
                    new
                    {
                        Id = lineId
                    });
            }
        }

        async public Task<TransferRequest> GetTransferRequest(string CodSF)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryOne<TransferRequest>(
                    "SELECT * FROM TransfersRequests WHERE CodSF = @Code",
                    new
                    {
                        Code = CodSF
                    });
            }
        }

        async public Task<long> SaveTransferRequest(TransferRequest transfer)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"INSERT INTO TransfersRequests (CardCode,Contacto,DocDate,DocDueDate,TaxDate,AlmacenOrigen,AlmacenDestino,SlpCode,ImlMeno,TipoPD,CodeSN,RazonSocial,DateStart,DateEnd,Ubicacion,Observaciones,NumOVSF,LlamadaServ,Anexo)
                    VALUES (@CardCode,@Contacto,@DocDate,@DocDueDate,@TaxDate,@AlmacenOrigen,@AlmacenDestino,@SlpCode,@ImlMeno,@TipoPD,@CodeSN,@RazonSocial,@DateStart,@DateEnd,@Ubicacion,@Observaciones,@NumOVSF,@LlamadaServ,@Anexo)",
                    transfer);

                return await provider.QueryOne<long>("SELECT MAX(Id) FROM TransfersRequests");
            }
        }

        async public Task<long> SaveTransferRequestsLine(TransferRequestDetail line)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"INSERT INTO TransfersRequestsDetail (ItemCode,Almacen,AlmacenDest,Quantity,TransferId)
                    VALUES (@ItemCode,@Almacen,@AlmacenDest,@Quantity,@TransferId)",
                    line);

                return await provider.QueryOne<long>("SELECT MAX(Id) FROM TransfersRequestsDetail");
            }
        }

        async public Task DeleteTransferRequest(long Id)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    "DELETE FROM TransfersRequests WHERE Id = @Id;DELETE FROM TransfersRequestsDetail WHERE TransferId = @Id",
                    new
                    {
                        Id
                    });
            }
        }
    }
}
