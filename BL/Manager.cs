using BDFA.Data;
using BDFA.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SQLite;
using System.Net;
using System.Net.Mail;

namespace BDFA.BL
{
    public class Manager
    {
        // local variables
        private static string _ConnectionString;
        private static string _SMTPFrom;
        private static string _SMTPFromName;
        private static string _SMTPUserName;
        private static string _SMTPPassword;
        private static string  _SMTPHost;
        private static int _SMTPPort = 25;
        private static bool _SMTPSSL = true;

        //global variables
        public static string SettingsProfileAbout { get; set; }

        public static void InitializeSMTPSettings(IConfiguration configuration)
        {
            _SMTPFrom = configuration["SMTPSettings:SMTPFrom"];
            _SMTPFromName = configuration["SMTPSettings:SMTPFromName"];
            _SMTPUserName = configuration["SMTPSettings:SMTPUserName"];
            _SMTPPassword = configuration["SMTPSettings:SMTPPassword"];
            _SMTPHost = configuration["SMTPSettings:SMTPHost"];
            _SMTPPort = Convert.ToInt32(configuration["SMTPSettings:SMTPPort"]);
            _SMTPSSL = Convert.ToBoolean(configuration["SMTPSettings:SMTPSSL"]);
        }

        public static void InitializeDBSettings(IConfiguration configuration)
        {
            _ConnectionString = configuration["ConnectionStrings:DirectoryContext"];
        }

        public static void InitializeSiteAdmin()
        {
            GetSiteSettings(1);
        }

        public static Setting GetSiteSettings(int id)
        {
            Setting record = null;
            using (var connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM Settings WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            record = new Setting
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                            };
                        }
                    }
                }
            }
            return record;
        }

        public static void SendMail(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_SMTPHost) || _SMTPPort == 0)
            {
                throw new InvalidOperationException("SMTP settings are not initialized.");
            }

            SmtpClient client = new(_SMTPHost, _SMTPPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_SMTPUserName, _SMTPPassword),
                EnableSsl = _SMTPSSL
            };

            MailMessage mailMessage = new()
            {
                From = new MailAddress(_SMTPFrom, _SMTPFromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            client.Send(mailMessage);
        }

        public static async Task<bool> IsAdminAsync(DirectoryContext context, string email)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));

            // Check if any admin has the specified email.
            return await context.Admins.AnyAsync(admin => admin.Email == email);
        }
    }
}
