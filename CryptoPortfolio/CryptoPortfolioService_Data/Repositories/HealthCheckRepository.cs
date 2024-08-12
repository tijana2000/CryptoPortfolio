using CryptoPortfolioService_Data.Entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Diagnostics;
using System.Linq;

namespace CryptoPortfolioService_Data.Repositories
{
    public class HealthCheckRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public HealthCheckRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("HealthCheckTable");
            _table.CreateIfNotExists();
        }

        public IQueryable<HealthCheck> RetrieveAllHealthChecks()
        {
            var results = from g in _table.CreateQuery<HealthCheck>()
                          where g.PartitionKey == "HealthCheck"
                          select g;
            return results;
        }

        public void AddHealthCheck(HealthCheck newHealthCheck)
        {
            try
            {
                TableOperation insertOperation = TableOperation.Insert(newHealthCheck);
                _table.Execute(insertOperation);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[HEALTH MONITORING SERVICE]: An error occurred when trying to add new Health Check:\n{ex.Message}");
            }
        }

        public IQueryable<HealthCheck> GetLatestHealthChecks(string service, string instance)
        {
            // Define the time threshold for the last 24 hours
            DateTime threshold = DateTime.UtcNow.Subtract(TimeSpan.FromHours(24));

            // Create a query filter to retrieve health checks for the specified service and within the last 24 hours
            string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "HealthCheck");
            string serviceFilter = TableQuery.GenerateFilterCondition("Service", QueryComparisons.Equal, service);
            string instanceFilter = TableQuery.GenerateFilterCondition("Instance", QueryComparisons.Equal, instance);
            string timestampFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, threshold);

            // Combine the filters using the 'And' operator
            string combinedFilter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, serviceFilter);
            combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.And, instanceFilter);
            combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.And, timestampFilter);

            // Create a query using the filter
            TableQuery<HealthCheck> query = new TableQuery<HealthCheck>().Where(combinedFilter);

            // Execute the query and return the results
            return _table.ExecuteQuery(query).AsQueryable();
        }

        public IQueryable<HealthCheck> GetWebRoleHealthChecksForLastHour()
        {
            // Define the time threshold for the last 24 hours
            DateTime oneHourAgo = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));

            // Create a query filter to retrieve health checks for the specified service and within the last 24 hours
            string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "HealthCheck");
            string serviceFilter = TableQuery.GenerateFilterCondition("Service", QueryComparisons.Equal, "WebRole");
            string instanceFilter = TableQuery.GenerateFilterCondition("Instance", QueryComparisons.Equal, "0");
            string timestampFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, oneHourAgo);

            // Combine the filters using the 'And' operator
            string combinedFilter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, serviceFilter);
            combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.And, instanceFilter);
            combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.And, timestampFilter);

            // Create a query using the filter
            TableQuery<HealthCheck> query = new TableQuery<HealthCheck>().Where(combinedFilter);

            // Execute the query and return the results
            return _table.ExecuteQuery(query).AsQueryable();
        }
    }
}
