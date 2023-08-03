using DAL.DataModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services;

namespace SendMessageToSBTopic
{
    public class SendMessageToSBTopic
    {
        private readonly ILogger _logger;
        private readonly NotificationService notificationService;

        public SendMessageToSBTopic(
            ILoggerFactory loggerFactory, 
            NotificationService noteService)
        {
            _logger = loggerFactory.CreateLogger<SendMessageToSBTopic>();
            notificationService = noteService;
        }

        [Function("SendMessageToSBTopic")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

            var filter = new Notification
            {
                IsDelivered = false,
                PickAndLock = false,
            };
            var notifications = await notificationService.GetAllNotifications(filter);

            if (notifications.Any())
            {
                notifications.
                   ForEach(msg => {
                       msg.IsDelivered = false; msg.DeliveredDT = null;
                       msg.PickAndLock = true; msg.PickAndLockDT = DateTime.UtcNow;
                   });

                await notificationService.BulkUpdateNotification(notifications);

                await ServiceBusMessageSender.SendBatchMessagesAsync(notifications);

                notifications.
                    ForEach(msg => { 
                        msg.IsDelivered = msg.IsDelivered; msg.DeliveredDT = msg.DeliveredDT;
                        msg.PickAndLock = false; msg.PickAndLockDT = null;
                        msg.IsRead = false; msg.ReadDT = null;
                    });
                await notificationService.BulkUpdateNotification(notifications);
            }

            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
