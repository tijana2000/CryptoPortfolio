using System;
using System.IO;
using Common;
using Newtonsoft.Json;

namespace ConfigurationManager
{
    class Program
    {
        private const string adminUsername = "admin";
        private const string adminPassword = "admin";
        public static EmailSender emailSender { get; set; }

        public static void Main(string[] args)
        {
            while (!Login()) { }
            emailSender = new EmailSender(31);
            ShowMenu();
        }

        private static bool Login()
        {
            Console.Clear();
            Console.WriteLine("Please login to continue.");

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = ReadPassword();

            if (username == adminUsername && password == adminPassword)
            {
                Console.WriteLine("\nLogin successful!");
                return true;
            }
            else
            {
                Console.WriteLine("\nInvalid username or password. Exiting...");
                return false;
            }
        }

        private static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info;
            do
            {
                info = Console.ReadKey(true);
                if (info.Key != ConsoleKey.Backspace && info.Key != ConsoleKey.Enter)
                {
                    password += info.KeyChar;
                    Console.Write("*");
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            } while (info.Key != ConsoleKey.Enter);
            return password;
        }

        private static void ShowMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("1. Change the config");
                Console.WriteLine("2. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ChangeConfig();
                        break;
                    case "2":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void ChangeConfig()
        {
            Console.Clear();
            Console.Write("Enter new HealthEmailUser: ");
            string newEmail = Console.ReadLine();

            if (IsValidEmail(newEmail))
            {
                emailSender.EmailSettings.HealthEmailUser = newEmail;
                WriteConfig(emailSender.EmailSettings);
                Console.WriteLine("Config updated successfully. Press any key to return to menu...");
            }
            else
            {
                Console.WriteLine("Invalid email format. Press any key to try again...");
            }
            Console.ReadKey();
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailCheck = new System.Net.Mail.MailAddress(email);
                return emailCheck.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static void WriteConfig(EmailSettings config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(emailSender.OriginalConfigFilePath, json);
        }
    }
}
