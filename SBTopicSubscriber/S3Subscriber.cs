using DAL.DataModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services;

namespace SBTopicSubscriber
{
    public class S3Subscriber
    {
        private readonly ILogger _logger;
        private readonly NotificationService genericService;

        public S3Subscriber(
           ILoggerFactory loggerFactory,
           NotificationService genService)
        {
            _logger = loggerFactory.CreateLogger<S3Subscriber>();
            genericService = genService;
        }

        //3. Correlation Filter
        [Function("OtherMessageSubscriber")]
        public async Task Run(
            [ServiceBusTrigger("topicmessage", "S3", Connection = "SBTopicConnectionString")] string mySbMsg,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId
            )
        {
            _logger.LogInformation($"START:OtherMessageSubscriber {DateTime.UtcNow}");
            _logger.LogInformation($"processed message: {mySbMsg}");

            try
            {
                var notice = new Notification().ToClassObj(mySbMsg);

                if (notice.Id > 0)
                {
                    notice.IsRead = true;
                    notice.ReadDT = DateTime.UtcNow;

                    await genericService.UpdateNotification(notice);
                }
            }
            catch (Exception)
            {
                throw new Exception($"Error: Move message Id: {messageId} to dead-letter");
            }
            _logger.LogInformation($"END:OtherMessageSubscriber {DateTime.UtcNow}");
        }


    }
}
