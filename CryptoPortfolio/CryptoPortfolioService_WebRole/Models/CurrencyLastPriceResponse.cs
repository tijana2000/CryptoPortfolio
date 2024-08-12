using Newtonsoft.Json;

namespace CryptoPortfolioService_WebRole.Models
{
    public class CurrencyLastPriceResponse
    {
        [JsonProperty(PropertyName = "lprice")]
        public string LastPrice { get; set; }

        [JsonProperty(PropertyName = "curr1")]
        public string Currency1 { get; set; }

        [JsonProperty(PropertyName = "curr2")]
        public string Currency2 { get; set; }
    }
}