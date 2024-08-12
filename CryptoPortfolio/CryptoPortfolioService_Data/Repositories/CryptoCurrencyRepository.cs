using CryptoPortfolioService_Data.Entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoPortfolioService_Data.Repositories
{
    public class CryptoCurrencyRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public CryptoCurrencyRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("CryptoCurrencyTable");
            _table.CreateIfNotExists();
        }

        public IQueryable<CryptoCurrency> RetrieveAllCryptoCurrencies()
        {
            var results = from g in _table.CreateQuery<CryptoCurrency>()
                          where g.PartitionKey == "CryptoCurrency"
                          select g;
            return results;
        }

        public bool Exists(string currencyName, string userId)
        {
            return RetrieveAllCryptoCurrencies()
                .Where(s => s.CurrencyName == currencyName && s.UserId == userId).FirstOrDefault() != null;
        }

        public CryptoCurrency RetrieveCurrencyForUser(string currencyName, string userId)
        {
            return RetrieveAllCryptoCurrencies().Where(c => c.CurrencyName == currencyName && c.UserId == userId).FirstOrDefault();            
        }

        public List<CryptoCurrency> RetrieveAllCurrenciesForUser(string userId)
        {
            return RetrieveAllCryptoCurrencies().Where(x => x.UserId == userId).ToList();
        }

        public void AddCryptoCurrency(CryptoCurrency newCryptoCurrency)
        {
            TableOperation insertOperation = TableOperation.Insert(newCryptoCurrency);
            _table.Execute(insertOperation);
        }

        public void UpdateCryptoCurrency(CryptoCurrency cryptoCurrency)
        {
            TableOperation updateOperation = TableOperation.Replace(cryptoCurrency);
            _table.Execute(updateOperation);
        }

        public void RemoveCryptoCurrency(string id)
        {
            CryptoCurrency cryptoCurrency = RetrieveAllCryptoCurrencies().Where(s => s.RowKey == id).FirstOrDefault();
            if (cryptoCurrency != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(cryptoCurrency);
                _table.Execute(deleteOperation);
            }
        }

        public bool UserHasEnoughCrypto(string currencyName, string userId, int quantity)
        {
            return RetrieveAllCryptoCurrencies()
                .Where(x => x.CurrencyName == currencyName && 
                       x.UserId == userId && 
                       x.Quantity >= quantity)
                .FirstOrDefault() != null;
        }
    }
}
