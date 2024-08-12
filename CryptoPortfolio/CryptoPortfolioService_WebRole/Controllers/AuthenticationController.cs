using CryptoPortfolioService_Data.BlobStorage;
using CryptoPortfolioService_Data.Entities;
using CryptoPortfolioService_Data.Repositories;
using CryptoPortfolioService_WebRole.Services;
using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CryptoPortfolioService_WebRole.Controllers
{
    public class AuthenticationController : Controller
    {
        UserRepository _userRepository = new UserRepository();
        BlobHelper _blobHelper = new BlobHelper();
        
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["userRowKey"] = null;
            Session["userType"] = null;
            return View("Login");
        }

        [HttpPost]
        public ActionResult AddEntity(string Name, string Surname,
                                      string Address, string City, string Country,
                                      string Phone, string Email, string Password,
                                      HttpPostedFileBase file)
        {
            try
            {
                if (_userRepository.IsEmailUnique(Email))
                {
                    return View("Error");
                }
                if (file == null)
                {
                    ViewBag.ErrorMsg = "Profile picture is required.";
                    return View("Error");
                }
                if(!Validator.ValidateUser(Name, Surname, Address, City, Country, Phone, Password))
                {
                    ViewBag.ErrorMsg = "One or more fields is not valid.";
                    return View("Error");
                }

                string generatedUserId = Guid.NewGuid().ToString();

                MemoryStream target = new MemoryStream();
                file.InputStream.CopyTo(target);
                byte[] imageByteArray = target.ToArray();

                string blobUri = _blobHelper.UploadProfileImage(generatedUserId, imageByteArray);

                User user = new User()
                {
                    RowKey = generatedUserId,
                    Name = Name,
                    Surname = Surname,
                    Address = Address,
                    City = City,
                    Country = Country,
                    Phone = Phone,
                    Email = Email,
                    Password = Password,                    
                    PhotoUrl = blobUri,
                    Type = CryptoPortfolioService_Data.Entities.Enums.UserType.VISITOR.ToString()
                };                
                
                _userRepository.AddUsear(user);
                return RedirectToAction("Login");
            }
            catch
            {
                return View("Register");    
            }
        }        

        [HttpPost]
        public ActionResult SetSession(string Email, string Password)
        {
            User user = _userRepository.GetUserByCredentials(Email, Password);
            if (user is null)
                return View("Login");

            Session["userRowKey"] = user.RowKey;
            Session["userType"] = user.Type;
            return RedirectToAction("Profile", "User");
        }
    }
}