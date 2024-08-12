using CryptoPortfolioService_Data.Entities;
using CryptoPortfolioService_Data.Repositories;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Common
{
    public class EmailSender : IEmailSender
    {
        public int NumberOfCharactersToRemove { get; set; }
        public string ConfigFolder { get; set; }
        public string ConfigFilePath { get; set; }
        public string OriginalConfigFilePath { get; set; }
        public EmailSettings EmailSettings { get; set; }

        public EmailSender(int numberOfCharactersToRemove)
        {
            NumberOfCharactersToRemove = numberOfCharactersToRemove;
            ConfigFolder = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - NumberOfCharactersToRemove);
            ConfigFilePath = Path.Combine($"{ConfigFolder}NotificationService_WorkerRole\\bin\\Debug\\appsettings.json");
            OriginalConfigFilePath = Path.Combine($"{ConfigFolder}NotificationService_WorkerRole\\appsettings.json");
            EmailSettings = LoadEmailSettings();
        }

        private EmailSettings LoadEmailSettings()
        {
            try
            {
                string json = File.ReadAllText(ConfigFilePath);
                return JsonConvert.DeserializeObject<EmailSettings>(json);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[EMAIL SENDER]: An error occurred when trying to read the configuration file: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SendNotificationEmail(Alarm alarm)
        {
            try
            {
                if (EmailSettings == null)
                {
                    Trace.WriteLine("[NOTIFICATION SERVICE]: An error occurred when trying to send email:\n\tFailed to read email settings from configuration file!");
                    return false;
                }

                var _userRepository = new UserRepository();
                User user = _userRepository.GetUser(alarm.UserId);
                if (user == null)
                {
                    Trace.WriteLine("[NOTIFICATION SERVICE]: An error occurred when trying to send email:\n\tUser not found!");
                    return false;
                }

                // Create client
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(EmailSettings.EmailUser, EmailSettings.AppPassword)
                };

                // Create the email message
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(EmailSettings.EmailUser);
                mail.To.Add(user.Email);

                var _cryptoCurrencyRepository = new CryptoCurrencyRepository();
                var cryptoCurrency = _cryptoCurrencyRepository.RetrieveCurrencyForUser(alarm.CurrencyName, alarm.UserId);

                // Fill in message
                mail.IsBodyHtml = true;
                var body = "<h1>Alarm Notification!</h1>";
                body += $"<h3><br>The alarm for {cryptoCurrency.CurrencyName} has been triggere!";
                body += $"<br>The profit threshold set was: <b>{alarm.Profit}</b>";
                body += $"<br>The current profit is: <b>{cryptoCurrency.Profit}</b></h3>";
                mail.Body = body;
                mail.Subject = $"{cryptoCurrency.CurrencyName} alarm activated";

                // Send the email
                await smtp.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[NOTIFICATION SERVICE]: An error occurred when trying to send email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendAlertEmail(HealthCheck healthCheck)
        {
            try
            {
                if (EmailSettings == null)
                {
                    Trace.WriteLine("[HEALTH MONITORING SERVICE]: An error occurred when trying to send email:\n\tFailed to read email settings from configuration file!");
                    return false;
                }

                // Create client
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(EmailSettings.EmailUser, EmailSettings.AppPassword)
                };

                // Create the email message
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(EmailSettings.EmailUser);
                mail.To.Add(EmailSettings.HealthEmailUser);

                // Fill in message
                mail.IsBodyHtml = true;
                var body = "<h1>Healt Alert!</h1>";
                body += $"<h3><br>Detected failure of the {healthCheck.Service} at {healthCheck.Timestamp.Date} : {healthCheck.Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}!";
                mail.Body = body;
                mail.Subject = $"Health Alert";

                // Send the email
                await smtp.SendMailAsync(mail);;
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[HEALTH MONITORING SERVICE]: An error occurred when trying to send email: {ex.Message}");
                return false;
            }
        }
    }

    public class EmailSettings
    {
        public string HealthEmailUser { get; set; }
        public string EmailUser { get; set; }
        public string AppPassword { get; set; }
    }
}
