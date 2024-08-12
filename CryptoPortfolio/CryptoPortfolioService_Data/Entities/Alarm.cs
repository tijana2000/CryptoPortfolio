using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CryptoPortfolioService_Data.Entities
{
    public class Alarm : TableEntity
    {
        public string UserId { get; set; }
        public string CurrencyName { get; set; }
        public double Profit { get; set; }

        public Alarm()
        {
            PartitionKey = "Alarm";
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
