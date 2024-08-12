using CryptoPortfolioService_Data.Entities.Enums;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CryptoPortfolioService_Data.Entities
{
    public class User : TableEntity
    {        
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhotoUrl { get; set; }
        public string Type { get; set; }

        public User()
        {
            PartitionKey = "User";
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
