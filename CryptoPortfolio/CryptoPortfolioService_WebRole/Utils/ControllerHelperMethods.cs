using CryptoPortfolioService_Data.Entities;
using CryptoPortfolioService_Data.Entities.Enums;
using CryptoPortfolioService_Data.Repositories;
using System.Web;

namespace CryptoPortfolioService_WebRole.Utils
{
    internal class ControllerHelperMethods
    {
        UserRepository _userRepository = new UserRepository();

        internal User GetUserFromSession()
        {            
            string userRowKey = (string)HttpContext.Current.Session["userRowKey"];
            if (userRowKey == null)
                return null;

            return _userRepository.GetUser(userRowKey);
        }

        internal bool LoggedInUserIsType(UserType type)
        {
            var user = GetUserFromSession();
            if (user == null || user.Type != type.ToString())
            {
                return false;
            }
            return true;
        }
    }
}