using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolioService_Data.Entities
{
    public class EndpointEntity : TableEntity
    {
        public EndpointEntity(string serviceName, string instanceId)
        {
            this.PartitionKey = serviceName;
            this.RowKey = instanceId;
        }

        public EndpointEntity() { }

        public string Url { get; set; }
    }
}
