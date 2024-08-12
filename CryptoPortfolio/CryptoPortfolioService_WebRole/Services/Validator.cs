namespace CryptoPortfolioService_WebRole.Services
{
    public static class Validator
    {
        public static bool ValidateUser(string Name, string Surname,
                                    string Address, string City, string Country,
                                    string Phone, string Password)
        {
            if (Name == null || Name.Length < 1)
                return false;

            if (Address == null || Address.Length < 1)
                return false;

            if (Surname == null || Surname.Length < 1)
                return false;

            if (City == null || City.Length < 1)
                return false;

            if (Country == null || Country.Length < 1)
                return false;

            if (Phone == null || Phone.Length < 1)
                return false;

            if (Password == null || Password.Length < 1)
                return false;

            return true; 
        }

        public static bool ValidateTransaction(int quantity, double price)
        {
            if (quantity < 1)
                return false;

            if (price <= 0)
                return false;

            return true;
        }
    }
}