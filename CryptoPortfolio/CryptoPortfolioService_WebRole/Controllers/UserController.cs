using CryptoPortfolioService_Data.BlobStorage;
using CryptoPortfolioService_Data.Entities;
using CryptoPortfolioService_Data.Repositories;
using CryptoPortfolioService_WebRole.Constants;
using CryptoPortfolioService_WebRole.Models;
using CryptoPortfolioService_WebRole.Services;
using CryptoPortfolioService_WebRole.Utils;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CryptoPortfolioService_WebRole.Controllers
{
    public class UserController : Controller
    {
        ControllerHelperMethods _helpers = new ControllerHelperMethods();
        UserRepository _userRepository = new UserRepository();
        BlobHelper _blobHelper = new BlobHelper();
        
        public ActionResult ChangePicture()
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            byte[] imageBytes = _blobHelper.DownloadImageToByteArray(user.RowKey);
            ImageModel imageModel = new ImageModel() { ImageBytes = imageBytes };

            return View(imageModel);
        }

        [HttpPost]
        public ActionResult UpdateBlob(HttpPostedFileBase file)
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            if (file == null)
            {
                ViewBag.ErrorMsg = "Profile picture is required.";
                return View("Error");
            }

            MemoryStream target = new MemoryStream();
            file.InputStream.CopyTo(target);
            byte[] imageByteArray = target.ToArray();

            user.PhotoUrl = _blobHelper.UploadProfileImage(user.RowKey, imageByteArray);            
            _userRepository.UpdateUser(user);

            return RedirectToAction("Profile");
        }

        public ActionResult Profile()
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            byte[] imageBytes = _blobHelper.DownloadImageToByteArray(user.RowKey);

            UserProfileModel userProfileModel = new UserProfileModel(user);
            userProfileModel.ImageBytes = imageBytes;            

            return View(userProfileModel);
        }

        public ActionResult EditProfile()
        {
            User user = _helpers.GetUserFromSession();
            if (user is null)
                return View(PathConstants.LoginViewPath);

            return View(user);
        }
       
        [HttpPost]
        public ActionResult ModifyEntity(string RowKey, string Name, string Surname,
                                         string Address, string City, string Country,
                                         string Phone, string Email, string OldPassword, string NewPassword)
        {
            try
            {
                if (!_userRepository.Exists(RowKey))
                {
                    return View("Error");
                }                

                User currentUser = _helpers.GetUserFromSession();
                if (currentUser is null)
                    return View(PathConstants.LoginViewPath);

                if (!_userRepository.IsPasswordValid(currentUser.Email, OldPassword))
                {
                    ViewBag.ErrorMsg = "Incorrect password";
                    return View("Error");
                }

                if (!Validator.ValidateUser(Name, Surname, Address, City, Country, Phone, NewPassword))
                {
                    ViewBag.ErrorMsg = "One or more fields is not valid.";
                    return View("Error");
                }

                User user = _userRepository.GetUser(RowKey);

                user.Name = Name;
                user.Surname = Surname;
                user.Address = Address;
                user.City = City;
                user.Country = Country;
                user.Phone = Phone;
                user.Email = Email;
                user.Password = NewPassword;
                user.PhotoUrl = "hardcoded";

                _userRepository.UpdateUser(user);

                return RedirectToAction("Profile");
            }
            catch (Exception e)
            {
                string exceptionMessage = e.Message;
                return View("Error");
            }            
        }                
    }
}