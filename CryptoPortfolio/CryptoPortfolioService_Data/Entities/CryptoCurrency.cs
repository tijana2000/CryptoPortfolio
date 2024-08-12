using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CryptoPortfolioService_Data.Entities
{
    public class CryptoCurrency : TableEntity
    {
        public string UserId { get; set; }
        public string CurrencyName { get; set; }
        public int Quantity { get; set; }
        public double Profit { get; set; }

        public CryptoCurrency()
        {
            PartitionKey = "CryptoCurrency";
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
