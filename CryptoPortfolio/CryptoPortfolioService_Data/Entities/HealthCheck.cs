using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CryptoPortfolioService_Data.Entities
{
    public class HealthCheck : TableEntity
    {
        public HealthCheck()
        {
            PartitionKey = "HealthCheck";
            RowKey = Guid.NewGuid().ToString();
        }

        public string Status { get; set; }
        public string Service { get; set; }
        public string Instance { get; set; }
    }
}
