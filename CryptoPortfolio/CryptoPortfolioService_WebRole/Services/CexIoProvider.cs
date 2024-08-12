using CryptoPortfolioService_WebRole.Constants;
using CryptoPortfolioService_WebRole.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace CryptoPortfolioService_WebRole.Services
{
    public class CexIOProvider
    {
        private HttpClient Client { get; set; }

        public CexIOProvider()
        {
            Client = new HttpClient();
        }

        public async Task<CurrencyLimitsResponse> GetCurrencyLimits()
        {
            HttpResponseMessage response = await Client.GetAsync(CexIoUrlConstants.GetLimitsUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CurrencyLimitsResponse>(responseBody);
        }

        public async Task<CurrencyLastPriceResponse> GetCurrencyLastPrice(string symbol1, string symbol2)
        {
            string url = CexIoUrlConstants.LastPriceUrl;
            string parameters = $"{symbol1}/{symbol2}";
            url += parameters;

            HttpResponseMessage response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CurrencyLastPriceResponse>(responseBody);
        }
    }
}