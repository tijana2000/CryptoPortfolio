using CryptoPortfolioService_Data.Entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoPortfolioService_Data.Repositories
{
    public class AlarmRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudQueue _queue;

        public AlarmRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudQueueClient queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("alarmqueue");
            _queue.CreateIfNotExists();
        }

        public async Task AddAlarmAsync(Alarm newAlarm)
        {
            string alarmMessage = JsonConvert.SerializeObject(newAlarm);
            CloudQueueMessage message = new CloudQueueMessage(alarmMessage);
            await _queue.AddMessageAsync(message);
        }

        public async Task<List<Alarm>> GetTopAlarmsAsync(int count = 20)
        {
            Thread.Sleep(new Random().Next(0, 10000)); // Ensures no two instances can get the same data because of timing
            var messages = await _queue.GetMessagesAsync(count);
            var alarms = new List<Alarm>();

            foreach (var message in messages)
            {
                var alarm = JsonConvert.DeserializeObject<Alarm>(message.AsString);
                alarms.Add(alarm);
                await _queue.DeleteMessageAsync(message); // Delete the message from the queue
            }

            return alarms;
        }

        public async Task RequeueAlarmAsync(Alarm alarm)
        {
            string alarmMessage = JsonConvert.SerializeObject(alarm);
            CloudQueueMessage message = new CloudQueueMessage(alarmMessage);
            await _queue.AddMessageAsync(message);
        }
    }
}
