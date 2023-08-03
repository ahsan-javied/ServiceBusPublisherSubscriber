using DAL.DataModels;
using DAL.Repository;

namespace Services
{
    public class NotificationService
    {
        private readonly INotificationRepository NotificationRepository;

        public NotificationService(INotificationRepository repo)
        {
            NotificationRepository = repo;
        }

        public async Task<List<Notification>> GetAllNotifications(Notification notification)
        {
            return await NotificationRepository.GetAllNotifications(notification);
        }

        public async Task UpdateNotification(Notification notification)
        {
            await NotificationRepository.UpdateNotification(notification);
        }

        public async Task BulkUpdateNotification(List<Notification> notifications)
        {
            await NotificationRepository.BulkUpdateNotification(notifications);
        }
    }
}
