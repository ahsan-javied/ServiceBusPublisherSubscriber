using System.Data;
using DAL.DataModels;

namespace DAL.Repository
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public async Task<List<Notification>> GetAllNotifications(Notification notification)
        {
            string sql = "SELECT * FROM Notifications where IsDelivered = @IsDelivered AND PickAndLock = @PickAndLock";

            return (List<Notification>)await GetAllByParms(sql, notification);
        }

        public async Task UpdateNotification(Notification notification)
        {
            string sql = "UPDATE Notifications SET IsDelivered = @IsDelivered, DeliveredDT = @DeliveredDT, IsRead = @IsRead, ReadDT = @ReadDT WHERE Id = @Id";

            if (notification.IsDelivered)
                sql = "UPDATE Notifications SET IsDelivered = @IsDelivered, DeliveredDT = @DeliveredDT WHERE Id = @Id";

            if (notification.IsRead)
                sql = "UPDATE Notifications SET IsRead = @IsRead, ReadDT = @ReadDT WHERE Id = @Id";


            await Update(sql, notification);
        }

        public async Task BulkUpdateNotification(List<Notification> notifications)
        {
            string sql = "UPDATE Notifications SET " +
                            "IsDelivered = @IsDelivered, DeliveredDT = @DeliveredDT, " +
                            "IsRead = @IsRead, ReadDT = @ReadDT, " +
                            "PickAndLock = @PickAndLock, PickAndLockDT= @PickAndLockDT " +
                            "WHERE Id = @Id";

            await BulkUpdate(sql, notifications);
        }
    }
}