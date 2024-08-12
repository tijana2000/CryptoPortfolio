using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoPortfolioService_WebRole.Models
{
    public class CurrencyLimitsResponse
    {
        [JsonProperty(PropertyName = "data")]
        public ApiData Data { get; set; }
    }

    public class ApiData
    {
        [JsonProperty(PropertyName = "pairs")]
        public List<CurrencyPairs> Pairs { get; set; }
    }

    public class CurrencyPairs
    {
        [JsonProperty(PropertyName = "symbol1")]
        public string Symbol1 { get; set; }

        [JsonProperty(PropertyName = "symbol2")]
        public string Symbol2 { get; set; }

        [JsonProperty(PropertyName = "minPrice")]
        public string MinPrice { get; set; }

        [JsonProperty(PropertyName = "maxPrice")]
        public string MaxPrice { get; set; }
    }
}