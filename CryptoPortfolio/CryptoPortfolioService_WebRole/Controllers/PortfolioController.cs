using CryptoPortfolioService_Data.Entities;
using CryptoPortfolioService_Data.Entities.Enums;
using CryptoPortfolioService_Data.Repositories;
using CryptoPortfolioService_WebRole.Constants;
using CryptoPortfolioService_WebRole.Services;
using CryptoPortfolioService_WebRole.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CryptoPortfolioService_WebRole.Controllers
{
    public class PortfolioController : Controller
    {        
        ControllerHelperMethods _helpers = new ControllerHelperMethods();     
        TransactionRepository _transactionRepository = new TransactionRepository();
        CryptoCurrencyRepository _cryptoCurrencyRepository = new CryptoCurrencyRepository();
        AlarmRepository _alarmRepository = new AlarmRepository();

        [HttpPost]
        public ActionResult AddTransactionEntity(string CurrencyName, int Quantity, double Price, bool IsBuying)
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
            transaction.CurrencyName = CurrencyName;
            transaction.Quantity = Quantity;
            transaction.PricePerUnit = Price;
            transaction.UserId = user.RowKey;

            if (IsBuying)
            {
                transaction.TransactionType = TransactionType.BUYING.ToString();                
            }
            else
            {
                transaction.TransactionType = TransactionType.SELLING.ToString();
                if (!_cryptoCurrencyRepository.UserHasEnoughCrypto(CurrencyName, user.RowKey, Quantity))
                {
                    ViewBag.ErrorMsg = $"You don't have enough {CurrencyName}.";
                    return View("Error");
                }
            }                                        

            AddOrUpdateCrptoCurrencyEntity(transaction, user.RowKey);
            _transactionRepository.AddTransaction(transaction);

            return RedirectToAction("CryptoCurrencies", "Crypto");
        }        

        [HttpPost]
        public ActionResult AddAlarmEntity(string currencyName, double profit)
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            Alarm alarm = new Alarm();
            alarm.CurrencyName = currencyName;
            alarm.Profit = profit;
            alarm.UserId = user.RowKey;

            _alarmRepository.AddAlarmAsync(alarm);
            return RedirectToAction("UserCryptoCurrencies");
        }

        public ActionResult Transactions()
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            List<Transaction> transactions = _transactionRepository.RetrieveAllTransactionsForUser(user.RowKey);
            List<Transaction> orderedTransactions = transactions.OrderBy(x => x.MadeOn).ToList();
            return View(orderedTransactions);
        }

        public ActionResult DeleteTransaction(string id)
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);
            
            Transaction transaction = _transactionRepository.RetrieveTransactionForUser(user.RowKey, id);
            if (!_transactionRepository.CanBeDeleted(transaction, user.RowKey))
            {
                ViewBag.ErrorMsg = "Only the last transaction for a specific currency can be deleted";
                return View("Error");
            }

            DeleteOrUpdateCryptoCurrencyEntity(transaction, user.RowKey);
            _transactionRepository.RemoveTransaction(id);
           
            return RedirectToAction("Transactions");
        }

        public ActionResult UserCryptoCurrencies()
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            List<CryptoCurrency> userCuurencies = _cryptoCurrencyRepository.RetrieveAllCurrenciesForUser(user.RowKey);
            return View(userCuurencies);
        }

        public ActionResult UserCurrencyDetails(string currencyName)
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            CryptoCurrency cryptoCurrency = _cryptoCurrencyRepository.RetrieveCurrencyForUser(currencyName, user.RowKey);
            return View(cryptoCurrency);
        }                

        private void AddOrUpdateCrptoCurrencyEntity(Transaction transaction, string userId)
        {
            if (_cryptoCurrencyRepository.Exists(transaction.CurrencyName, userId))
            {
                CryptoCurrency cryptoCurrency = _cryptoCurrencyRepository.RetrieveCurrencyForUser(transaction.CurrencyName, userId);
                if (transaction.TransactionType == TransactionType.BUYING.ToString())
                {
                    cryptoCurrency.Quantity += transaction.Quantity;
                    cryptoCurrency.Profit -= transaction.PricePerUnit * transaction.Quantity;
                }
                else
                {                    
                    cryptoCurrency.Quantity -= transaction.Quantity;
                    cryptoCurrency.Profit += transaction.PricePerUnit * transaction.Quantity;
                }

                _cryptoCurrencyRepository.UpdateCryptoCurrency(cryptoCurrency);
            }
            else
            {
                CryptoCurrency cryptoCurrency = new CryptoCurrency();
                cryptoCurrency.UserId = userId;
                cryptoCurrency.Quantity = transaction.Quantity;
                cryptoCurrency.CurrencyName = transaction.CurrencyName;
                cryptoCurrency.Profit = 0 - transaction.PricePerUnit * transaction.Quantity;

                _cryptoCurrencyRepository.AddCryptoCurrency(cryptoCurrency);
            }
        }

        private void DeleteOrUpdateCryptoCurrencyEntity(Transaction transaction, string userId)
        {
            CryptoCurrency cryptoCurrency = _cryptoCurrencyRepository.RetrieveCurrencyForUser(transaction.CurrencyName, userId);
            if (transaction.TransactionType == TransactionType.BUYING.ToString())
            {                
                cryptoCurrency.Quantity -= transaction.Quantity;
                cryptoCurrency.Profit += transaction.PricePerUnit * transaction.Quantity;

                _cryptoCurrencyRepository.UpdateCryptoCurrency(cryptoCurrency);                
            }
            else
            {
                cryptoCurrency.Quantity += transaction.Quantity;
                cryptoCurrency.Profit -= transaction.PricePerUnit * transaction.Quantity;

                _cryptoCurrencyRepository.UpdateCryptoCurrency(cryptoCurrency);
            }
        }
    }
}