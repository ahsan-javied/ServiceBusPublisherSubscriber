using DAL.DataModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services;

namespace SBTopicSubscriber
{
    public class S1Subscriber
    {
        private readonly ILogger _logger;
        private readonly NotificationService notificationService;

        public S1Subscriber(
           ILoggerFactory loggerFactory,
           NotificationService noteService)
        {
            _logger = loggerFactory.CreateLogger<S1Subscriber>();
            notificationService = noteService;
        }
        //1. Default(Boolean) Filter: Default = true
        [Function("AllMessageSubscriber")]
        public async Task RunAsync(
            [ServiceBusTrigger(topicName: "%SBTopicName%", subscriptionName: "%SBTopicSubscriptionName1%", Connection = "SBConnectionString")] string mySbMsg,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId
            )
        {
            _logger.LogInformation($"START:AllMessageSubscriber {DateTime.UtcNow}");
            _logger.LogInformation($"processed message: {mySbMsg}");
            
            try
            {
                var notice = new Notification().ToClassObj(mySbMsg);

                if (notice?.Id > 0)
                {
                    notice.IsRead = true;
                    notice.ReadDT = DateTime.UtcNow;

                    await notificationService.UpdateNotification(notice);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Exception:{ex}");
                throw new Exception($"Error: Move message Id: {messageId} to dead-letter");
            }
            _logger.LogInformation($"END:AllMessageSubscriber {DateTime.UtcNow}");
        }


    }
}
