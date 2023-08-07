
using DAL.DataModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services;
using Services.ServiceBus;
using System.Text;
using Utils.Settings;

namespace SBDLQProcessor
{
    public class DLQProcessor
    {
        private readonly ILogger _logger;
        private readonly ServiceBusMessageReceiver serviceBusMessageReceiver;
        private readonly NotificationService notificationService;

        public DLQProcessor(ILoggerFactory loggerFactory, ServiceBusMessageReceiver receiver, NotificationService notificationService)
        {
            _logger = loggerFactory.CreateLogger<DLQProcessor>();
            serviceBusMessageReceiver = receiver;
            this.notificationService = notificationService;
        }

        [Function("DLQProcessor")]
        public async Task RunAsync([TimerTrigger("0 */1 * * * *")] TimeTriggerInfo myTimer)
        {

            _logger.LogInformation($"START:S1 DLQProcessor {DateTime.UtcNow}");
            var messages = await serviceBusMessageReceiver.GetDLQMessageAsync();

            if (messages.Any())
            {
                var notifications = new List<Notification>();

                messages.
                   ForEach(msg =>
                   {
                       string messageBody = Encoding.UTF8.GetString(msg.Body);
                       var notice = new Notification().ToClassObj(messageBody);
                       
                       _logger.LogInformation($"Message from DLQ: {messageBody}");

                       if (notice?.Id > 0)
                       {
                           notice.PickAndLock = true;
                           notice.PickAndLockDT = DateTime.UtcNow;
                           notifications.Add(notice);
                       }
                   });

                await notificationService.BulkUpdateNotification(notifications);
            }

            _logger.LogInformation($"END:S1 DLQProcessor {DateTime.UtcNow}");
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }
}
