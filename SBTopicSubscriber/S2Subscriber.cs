using DAL.DataModels;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services;

namespace SBTopicSubscriber
{
    public class S2Subscriber
    {
        private readonly ILogger _logger;
        private readonly NotificationService genericService;

        public S2Subscriber(
           ILoggerFactory loggerFactory,
           NotificationService genService)
        {
            _logger = loggerFactory.CreateLogger<S2Subscriber>();
            genericService = genService;
        }

        //2. SQL Filter: role=admin
        [Function("AdminMessageSubscriber")]
        public async Task Run(
            [ServiceBusTrigger("topicmessage", "S2", Connection = "SBTopicConnectionString")] string mySbMsg,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId
            )
        {
            _logger.LogInformation($"START:AdminMessageSubscriber {DateTime.UtcNow}");
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
            _logger.LogInformation($"END:AdminMessageSubscriber {DateTime.UtcNow}");
        }


    }
}
