using System;
using System.CodeDom;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using FileProcessingService;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using ZXing;

namespace ScanerProcessingService
{
    public class MessagingClient
    {
        private const string queueName = "ScannerQueue3";
        private const string InfoQueueName = "ServiceStatus";
        private const string SettingsTopicName = "SettingsTopic2";
        private readonly string SubsName = "ScannerSettings";
        private const int maxMessageLength = 260000;

        public MessagingClient()
        {
            var nsManager = NamespaceManager.Create();
            
            if (!nsManager.SubscriptionExists(SettingsTopicName, SubsName))
            {
                var subscription = new SubscriptionDescription(SettingsTopicName, SubsName);
                nsManager.CreateSubscription(subscription);
            }
            if (!nsManager.QueueExists(queueName))
            {
                nsManager.CreateQueue(queueName);

            }
        }

        public void SendMessage(byte[] file)
        {
            var queueClient = QueueClient.Create(queueName);
            var numberOfChunck = file.Length / maxMessageLength + 1;
            for (var i = 0; i < numberOfChunck; i++)
            {
                byte[] chunk = file.Skip(i * maxMessageLength).Take(maxMessageLength).ToArray();
                Record record = new Record { data = chunk };
                BrokeredMessage recordMessage = new BrokeredMessage(record);
                recordMessage.Properties.Add("EnqueuedSequenceNumber", i);

                queueClient.Send(recordMessage);
            }

            queueClient.Close();
        }

        public Result ReadSettings()
        {
            var client = SubscriptionClient.Create(SettingsTopicName, SubsName, ReceiveMode.PeekLock);
            var message = client.Receive();
            Result data = null;
            if (message != null)
            {
                try
                {
                    SettingsMessage settingsMessage = message.GetBody<SettingsMessage>();
                    data = new Result(settingsMessage.Text, null, settingsMessage.ResultPoints, settingsMessage.Format);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return data;
        }

        public void SendStatus(string status)
        {
            var client = QueueClient.Create(InfoQueueName);
            client.Send(new BrokeredMessage(status));
            client.Close();
        }
    }

}