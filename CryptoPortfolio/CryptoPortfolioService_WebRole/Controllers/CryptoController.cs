using CryptoPortfolioService_Data.Entities;
using CryptoPortfolioService_Data.Repositories;
using CryptoPortfolioService_WebRole.Constants;
using CryptoPortfolioService_WebRole.Models;
using CryptoPortfolioService_WebRole.Services;
using CryptoPortfolioService_WebRole.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CryptoPortfolioService_WebRole.Controllers
{
    public class CryptoController : Controller
    {        
        ControllerHelperMethods _helpers = new ControllerHelperMethods();        
        TransactionRepository _transactionRepository = new TransactionRepository();
        CexIOProvider _cexIoProvider = new CexIOProvider();

        public async Task<ActionResult> CryptoCurrencies()
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            CurrencyLimitsResponse model = await GetCurrencyLimits();
            List<CurrencyPairListItem> viewModel = new List<CurrencyPairListItem>();

            model.Data.Pairs.ForEach(pairs =>
            {
                if (pairs.Symbol1 != "USD" && pairs.Symbol2 == "USD")
                    viewModel.Add(new CurrencyPairListItem(pairs.Symbol1, pairs.Symbol2, pairs.MinPrice, pairs.MaxPrice));
            });

            return await Task.Run(() => View(viewModel));
        }

        public async Task<ActionResult> CurrencyDetails(string symbol1, string symbol2, string minPrice, string maxPrice)
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            CurrencyLastPriceResponse lastPrice = await GetCurrencyLastPrice(symbol1, symbol2);
            CurrencyDetailsModel currencyDetails = new CurrencyDetailsModel(symbol1, symbol2, minPrice, maxPrice, lastPrice.LastPrice);

            return await Task.Run(() => View(currencyDetails));            
        }

        [HttpPost]
        public ActionResult AddEntity(string Symbol1, int Quantity, double Price)
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            if (!Validator.ValidateTransaction(Quantity, Price))
            {
                ViewBag.ErrorMsg = $"Invalid request.";
                return View("Error");
            }

            Transaction transaction = new Transaction();
            transaction.CurrencyName = Symbol1;
            transaction.Quantity = Quantity;
            transaction.PricePerUnit = Price;
            transaction.UserId = user.RowKey;

            _transactionRepository.AddTransaction(transaction);

            return View("Error");
        }        
        private async Task<CurrencyLimitsResponse> GetCurrencyLimits()
            => await _cexIoProvider.GetCurrencyLimits();

        private async Task<CurrencyLastPriceResponse> GetCurrencyLastPrice(string symbol1, string symbol2)
            => await _cexIoProvider.GetCurrencyLastPrice(symbol1, symbol2);
    }
}