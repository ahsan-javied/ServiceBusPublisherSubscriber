using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<List<Notification>> GetAllNotifications(Notification notification);
        Task UpdateNotification(Notification notification);
        Task BulkUpdateNotification(List<Notification> notifications);
    }
}
