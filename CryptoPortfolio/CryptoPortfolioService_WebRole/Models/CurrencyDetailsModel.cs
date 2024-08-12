namespace CryptoPortfolioService_WebRole.Models
{
    public class CurrencyDetailsModel
    {
        public CurrencyDetailsModel(string symbol1, string symbol2, string minPrice, string maxPrice, string lastPrice)
        {
            Symbol1 = symbol1;
            Symbol2 = symbol2;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            LastPrice = lastPrice;
        }
        public string Symbol1 { get; set; }
        public string Symbol2 { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public string LastPrice { get; set; }
    }
}