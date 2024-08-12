using CryptoPortfolioService_Data.Entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoPortfolioService_Data.Repositories
{
    public class UserTransactionRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public UserTransactionRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTransactionTable");
            _table.CreateIfNotExists();
        }

        public List<UserTransaction> RetrieveAllTransactions()
        {
            var results = from g in _table.CreateQuery<UserTransaction>()
                          where g.PartitionKey == "UserTransaction"
                          select g;
            return results.ToList();
        }

        public void AddTransaction(UserTransaction newTransaction)
        {
            TableOperation insertOperation = TableOperation.Insert(newTransaction);
            _table.Execute(insertOperation);
        }

        public bool TransactionExists(string id)
        {
            return RetrieveAllTransactions().Where(s => s.RowKey == id).FirstOrDefault() != null;
        }

        public UserTransaction GetTransaction(string id)
        {
            return RetrieveAllTransactions().Where(p => p.RowKey == id).FirstOrDefault();
        }

        public List<UserTransaction> GetTransactionByUser(string userEmail)
        {
            return RetrieveAllTransactions().Where(p => p.ReceiverEmail == userEmail || p.SenderEmail == userEmail).ToList();
        }

        public void RemoveTransaction(string id)
        {
            UserTransaction transaction = RetrieveAllTransactions().Where(s => s.RowKey == id).FirstOrDefault();
            if (transaction != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(transaction);
                _table.Execute(deleteOperation);
            }
        }

        public void UpdateTransaction(UserTransaction transaction)
        {
            TableOperation updateOperation = TableOperation.Replace(transaction);
            _table.Execute(updateOperation);
        }
    }
}
