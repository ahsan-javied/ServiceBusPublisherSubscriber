using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Services.ServiceBus
{
    public class ServiceBusMessageReceiver
    {
        public ServiceBusMessageReceiver(IConfiguration config)
        {
            Initialize(config);
        }

        private void Initialize(IConfiguration config)
        {
            sbConnectionString = config["SBConnectionString"] ?? "";
            topicName = config["SBTopicName"] ?? "";
            var subscriptionsName = config["SBTopicSubscriptions"] ?? "";
            Subscriptions = subscriptionsName?.Split(",") ?? Array.Empty<string>();
            int.TryParse(config["SBMaxNumOfMessages"], out numOfMessages);
        }

        // the client that owns the connection and can be used to create senders and receivers
        private static ServiceBusClient client;
        // the sender used to publish messages to the topic
        private static ServiceBusReceiver receiver;
        // connection string of the Service Bus
        private static string sbConnectionString;
        // name of the Service Bus topic
        private static string topicName;
        // names of subscriptions to the topic
        private static string[] Subscriptions;
        // number of messages to be sent to the topic
        private static int numOfMessages;

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


        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.

        public async Task<List<ServiceBusReceivedMessage>> GetDLQMessageAsync()
        {
            var messages = new List<ServiceBusReceivedMessage>();
            try
            {
                client = new ServiceBusClient(sbConnectionString);

                foreach (var subscription in Subscriptions)
                {
                    receiver = client.CreateReceiver(topicName, subscription + "/$DeadLetterQueue");
                    //ServiceBusReceiver dlqReceiver = client.CreateReceiver(topicName, Subscriptions[0] + "/$DeadLetterQueue", 
                    //    new ServiceBusReceiverOptions
                    //    {
                    //        ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete,
                    //    });

                    while (true)
                    {
                        try
                        {
                            // Receive a batch of messages from the dead-letter queue
                            var receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 10);

                            // No more messages in the dead-letter queue, exit the loop
                            if (receivedMessages.Count == 0)
                                break;

                            // Process the received messages
                            foreach (ServiceBusReceivedMessage message in receivedMessages)
                            {
                                // Handle the dead-lettered message
                                Console.WriteLine($"Dead-lettered message: {message.Body}");

                                // Complete the message to remove it from the dead-letter queue
                                await receiver.CompleteMessageAsync(message);
                                messages.Add(message);
                            }
                        }
                        catch (ServiceBusException ex)
                        {
                            // Handle any exceptions that occur during message processing
                            Console.WriteLine($"Error occurred while processing messages: {ex.Message}");
                        }
                    }
                    await receiver.DisposeAsync();
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
                await receiver.DisposeAsync();
                await client.DisposeAsync();
            }
            return messages;
        }
    }
}


