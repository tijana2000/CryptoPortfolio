using CryptoPortfolioService_Data.Entities;

namespace CryptoPortfolioService_WebRole.Models
{
    public class UserProfileModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[] ImageBytes { get; set; }

        public UserProfileModel(User user)
        {
            Name = user.Name;
            Surname = user.Surname;
            Address = user.Address;
            City = user.City;
            Country = user.Country;
            Phone = user.Phone;
            Email = user.Email;
            Password = user.Password;
        }
    }
}