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
        public static string DealImage1 { get; set; }
        public static string DealURL1 { get; set; }
        public static string DealImage2 { get; set; }
        public static string DealURL2 { get; set; }
        public static string DealImage3 { get; set; }
        public static string DealURL3 { get; set; }

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

        public static void InitializeSiteAdmin(IConfiguration configuration)
        {
            GetSiteSettings(Convert.ToInt32(configuration["Settings:SiteID"]));
        }

        public static Setting GetSiteSettings(int id)
        {
            Setting record = null;
            using (var connection = new SQLiteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM Settings WHERE Id = " + id, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DealImage1 = reader["DealImage1"].ToString();
                            DealURL1 = reader["DealURL1"].ToString();
                            DealImage2 = reader["DealImage2"].ToString();
                            DealURL2 = reader["DealURL2"].ToString();
                            DealImage3 = reader["DealImage3"].ToString();
                            DealURL3 = reader["DealURL3"].ToString();
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
