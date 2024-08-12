using CryptoPortfolioService_Data.Entities.Enums;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolioService_Data.Entities
{
    public class Transaction : TableEntity
    {
        public string UserId { get; set; }
        public string CurrencyName { get; set; }
        public double PricePerUnit { get; set; }
        public int Quantity { get; set; }
        public string TransactionType { get; set; }
        public DateTime MadeOn { get; set; }

        public Transaction()
        {
            PartitionKey = "Transaction";
            RowKey = Guid.NewGuid().ToString();
            MadeOn = DateTime.Now;
        }
    }
}
