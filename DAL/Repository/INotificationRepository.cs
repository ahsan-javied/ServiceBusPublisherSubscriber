using DAL.DataModels;

namespace DAL.Repository
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<List<Notification>> GetAllNotifications(Notification notification);
        Task UpdateNotification(Notification notification);
        Task BulkUpdateNotification(List<Notification> notifications);
    }
}
