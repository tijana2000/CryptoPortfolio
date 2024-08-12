using CryptoPortfolioService_Data.Entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoPortfolioService_Data.Queue
{
    public class NotificationQueueManager
    {
        private CloudStorageAccount _storageAccount;
        private CloudQueueClient _queueClient;
        private CloudQueue _queue;

        public NotificationQueueManager()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = _queueClient.GetQueueReference("alarmsdone");
            _queue.CreateIfNotExists();
        }

        public async Task<bool> AddAlarmIdsToQueue(List<string> alarmIds)
        {
            try
            {
                foreach (string alarmId in alarmIds)
                {
                    CloudQueueMessage message = new CloudQueueMessage(alarmId);
                    await _queue.AddMessageAsync(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                return false;
            }
        }

        public async Task<List<string>> GetAlarmIdsFromQueue()
        {
            try
            {
                List<string> alarmIds = new List<string>();

                CloudQueueMessage retrievedMessage = await _queue.GetMessageAsync();
                while (retrievedMessage != null)
                {
                    alarmIds.Add(retrievedMessage.AsString);

                    // Delete the message from the queue
                    await _queue.DeleteMessageAsync(retrievedMessage);

                    // Get the next message
                    retrievedMessage = await _queue.GetMessageAsync();
                }

                return alarmIds;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                return null;
            }
        }

        public async Task<bool> PersistAlarmNotification(DateTime timestamp, string alarmRowKey, int numberOfEmailsSent)
        {
            try
            {
                CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("AlarmNotifications");
                await table.CreateIfNotExistsAsync();

                AlarmNotification notificationEntity = new AlarmNotification(timestamp, alarmRowKey, numberOfEmailsSent);
                TableOperation insertOperation = TableOperation.Insert(notificationEntity);
                await table.ExecuteAsync(insertOperation);

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                return false;
            }
        }
    }
}
