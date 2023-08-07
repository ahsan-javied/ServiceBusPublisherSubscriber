using DAL.DataModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services;
using Services.ServiceBus;
using Utils.Settings;

namespace SendMessageToSBTopic
{
    public class SendMessageToSBTopic
    {
        private readonly ILogger _logger;
        private readonly NotificationService notificationService;
        private readonly ServiceBusMessageSender serviceBusMessageSender;
        
        public SendMessageToSBTopic(
            ILoggerFactory loggerFactory, 
            NotificationService noteService,
            ServiceBusMessageSender sbMessageSender)
        {
            _logger = loggerFactory.CreateLogger<SendMessageToSBTopic>();
            notificationService = noteService;
            serviceBusMessageSender = sbMessageSender;
        }

        [Function("SendMessageToSBTopic")]
        public async Task RunAsync([TimerTrigger("0 */1 * * * *")] TimeTriggerInfo myTimer)
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

                await serviceBusMessageSender.SendBatchMessagesAsync(notifications);

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
}
