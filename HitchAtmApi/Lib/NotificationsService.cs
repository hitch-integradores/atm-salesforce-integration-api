using System.Threading.Tasks;
using System.Collections.Generic;
using HitchAtmApi.Models;

namespace HitchAtmApi.Lib
{
    public class NotificationsService
    {
        ConnectionParameters DefaultConnectionParameters;

        public NotificationsService(ConnectionParameters connectionParameters)
        {
            DefaultConnectionParameters = connectionParameters;
        }

        async public Task<long> AddNotification(Notification notification)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"INSERT INTO Notifications
                    (RefId, RefType, Status, CreateTime, FinishTime, Steps, Stage)
                    VALUES (@RefId, @RefType, @Status, @CreateTime, @FinishTime, @Steps, @Stage)",
                    notification);

                return await provider.QueryOne<int>("SELECT MAX(Id) FROM notifications");
            }
        }

        async public Task UpdateNotification(Notification notification)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"UPDATE Notifications SET Status = @Status, RefId = @RefId, FinishTime = @FinishTime, Steps = @Steps, Stage = @Stage,
                    DocNum = @DocNum, DocEntry = @DocEntry WHERE Id = @Id",
                    notification);
            }
        }

        async public Task<Notification> GetNotification(long id, string refType)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryOne<Notification>(
                    @"SELECT * FROM Notifications WHERE RefId = @Id AND RefType = @RefType",
                    new
                    {
                        Id = id,
                        RefType = refType
                    });
            }
        }

        async public Task<List<Notification>> GetNotifications(List<string> refIds, string refType)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryAll<Notification>(string.Format(
                    @"SELECT Notifications.* FROM Notifications WHERE Notifications.RefId IN ({0})
                    AND Notifications.RefType = '{1}'",
                    string.Join(",", refIds.ToArray()), refType));
            }
        }

        async public Task Log(NotificationLog log)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                await provider.ExecCommand(
                    @"INSERT INTO NotificationsLogs
                    (CreateTime, Status, Operation, Message, NotificationId)
                    VALUES (@CreateTime, @Status, @Operation, @Message, @NotificationId)",
                    log);
            }
        }

        async public Task<List<NotificationLog>> GetNotificationLogs(long refId, List<string> subRefIds)
        {
            using (Database provider = new Database(DefaultConnectionParameters))
            {
                return await provider.QueryAll<NotificationLog>(string.Format(
                    @"SELECT * FROM NotificationsLogs WHERE NotificationId IN ({0},{1}) ORDER BY Id ASC",
                    refId, string.Join(",", subRefIds.ToArray())));
            }
        }
    }
}
