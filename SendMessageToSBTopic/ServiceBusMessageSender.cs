using Azure.Messaging.ServiceBus;
using DAL.DataModels;
using Grpc.Core;
using System.Text;
using Utils.Enums;
using Utils.Helpers;

namespace SendMessageToSBTopic
{
    public class ServiceBusMessageSender
    {
        // the client that owns the connection and can be used to create senders and receivers
        private static ServiceBusClient client;
        // the sender used to publish messages to the topic
        private static ServiceBusSender sender;
        // name of the Service Bus topic
        static readonly string topicName = "topicmessage";
        // connection string of the Service Bus
        static readonly string connectionString = "Endpoint=sb://mywork-messaging.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=qb7JPpE4Vz+Wfhh94nigrz593ePr/co7f+ASbADl92A=";
        
        // names of subscriptions to the topic
        static readonly string[] Subscriptions = { "S1", "S2", "S3" };
        static readonly IDictionary<string, string[]> SubscriptionFilters = new Dictionary<string, string[]> {
            { "S1", new[] { "StoreId IN('Store1', 'Store2', 'Store3')", "StoreId = 'Store4'"} },
            { "S2", new[] { "sys.To IN ('Store5','Store6','Store7') OR StoreId = 'Store8'" } },
            { "S3", new[] { "sys.To NOT IN ('Store1','Store2','Store3','Store4','Store5','Store6','Store7','Store8') OR StoreId NOT IN ('Store1','Store2','Store3','Store4','Store5','Store6','Store7','Store8')" } }
        };
        // You can have only have one action per rule and this sample code supports only one action for the first filter which is used to create the first rule. 
        static readonly IDictionary<string, string> SubscriptionAction = new Dictionary<string, string> {
            { "S1", "" },
            { "S2", "" },
            { "S3", "SET sys.Label = 'SalesEvent'"  }
        };
        static readonly string[] Store = { "Store1", "Store2", "Store3", "Store4", "Store5", "Store6", "Store7", "Store8", "Store9", "Store10" };
        //static string SysField = "sys.To";
        //static string CustomField = "StoreId";
        static readonly int NrOfMessagesPerStore = 1; // Send at least 1.

        // number of messages to be sent to the topic
        const int numOfMessages = 3;

        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.

        public static async Task SendBatchMessagesAsync(List<Notification> notifications)
        {
            try
            {
                client = new ServiceBusClient(connectionString);
                sender = client.CreateSender(topicName);

                // create a batch 
                List<List<Notification>> batches = ListHelper.SplitIntoBatches(notifications, numOfMessages);
            
                foreach (var batch in batches)
                {
                    using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

                    foreach (var message in batch)
                    {
                        var userRole = Enum.GetName(typeof(Enumerations.UserRole), message.UserRoleTypeId)?.ToLower();
                        var json = message.ToJsonString();
                        var sbMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
                        {
                            MessageId = Guid.NewGuid().ToString(),
                            ContentType= "application/json",
                            SessionId = message.UserId.ToString(),
                            //SQL Filter: role=admin & also for Correlation Filter
                            ApplicationProperties =
                            {
                                { "role", userRole},
                            }
                        };

                        //Correlation Filter
                        sbMessage.CorrelationId = userRole;
                        sbMessage.ReplyTo = userRole;

                        // try adding a message to the batch
                        if (!messageBatch.TryAddMessage(sbMessage))
                        {
                            message.PickAndLock = false;
                            message.PickAndLockDT = null;
                            
                            // if it is too large for the batch
                            Console.WriteLine($"The message {json} is too large to fit in the batch.");
                        }
                    }
                    try
                    {
                        // Use the producer client to send the batch of messages to the Service Bus topic
                        await sender.SendMessagesAsync(messageBatch);
                        Console.WriteLine($"A batch of {messageBatch.Count} messages has been published to the topic.");
                        messageBatch.Dispose();

                        batch.Where(msg => msg.PickAndLock).ToList()
                            .ForEach(msg => { msg.IsDelivered = true;  msg.DeliveredDT = DateTime.UtcNow; });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}


