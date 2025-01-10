using BDFA.Data;
using BDFA.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace BDFA.BL
{
    public class Manager
    {
        //private readonly DirectoryContext _context;

        //public Manager(DirectoryContext context)
        //{
        //    _context = context;
        //}

        // Local variables for SMTP and database settings
        private static string _ConnectionString;
        private static string _SMTPFrom;
        private static string _SMTPFromName;
        private static string _SMTPUserName;
        private static string _SMTPPassword;
        private static string _SMTPHost;
        private static int _SMTPPort = 25;
        private static bool _SMTPSSL = true;

        // Global variables for deal images and URLs
        public static string DealImage1 { get; set; }
        public static string DealURL1 { get; set; }
        public static string DealImage2 { get; set; }
        public static string DealURL2 { get; set; }
        public static string DealImage3 { get; set; }
        public static string DealURL3 { get; set; }

        // Generate a one-time code for authentication
        public static string GenerateOneTimeCode()
        {
            Random random = new();
            string _authCode = random.Next(100000, 999999).ToString();
            return _authCode;
        }

        // Initialize SMTP settings from configuration
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

        // Initialize database settings from configuration
        public static void InitializeDBSettings(IConfiguration configuration)
        {
            _ConnectionString = configuration["ConnectionStrings:DirectoryContext"];
        }

        // Initialize site admin settings from configuration
        public static void InitializeSiteAdmin(IConfiguration configuration)
        {
            // Retrieve the site ID from the appsettings.json file
            var _ConfigSiteID = Convert.ToInt32(configuration["Settings:SiteID"]);
            // Get the site settings
            _ = GetSiteSettingsAsync(_ConfigSiteID);
        }

        // Get site settings from the database
        public static async Task<Setting> GetSiteSettingsAsync(int id)
        {
            Setting record = null;
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                await connection.OpenAsync();
                using var command = new SqliteCommand("SELECT * FROM Settings WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    DealImage1 = reader["DealImage1"].ToString();
                    DealURL1 = reader["DealURL1"].ToString();
                    DealImage2 = reader["DealImage2"].ToString();
                    DealURL2 = reader["DealURL2"].ToString();
                    DealImage3 = reader["DealImage3"].ToString();
                    DealURL3 = reader["DealURL3"].ToString();
                }
            }
            return record;
        }

        // Send an email using SMTP settings
        public static bool SendMail(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_SMTPHost) || _SMTPPort == 0)
            {
                throw new InvalidOperationException("SMTP settings are not initialized.");
            }

            try
            {
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
                return true; // Email sent successfully
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false; // Email sending failed
            }
        }

        // Check if an admin exists by email
        public static async Task<bool> IsAdmin(DirectoryContext context, string email)
        {
            var admin = await context.Admins
                         .Where(u => u.Email == email)
                         .FirstOrDefaultAsync();
            return admin != null;
        }

        // Change the active status of a profile
        public static async Task ChangeProfileStatusAsync(int ProfileID, bool Active)
        {
            using var connection = new SqliteConnection(_ConnectionString);
            await connection.OpenAsync();
            using var command = new SqliteCommand("UPDATE Profiles SET Active = @Active WHERE id = @ProfileID", connection);
            command.Parameters.AddWithValue("@Active", Active);
            command.Parameters.AddWithValue("@ProfileID", ProfileID);
            await command.ExecuteNonQueryAsync();
        }

        // Change the featured author status of a profile
        public static async Task ChangeFeaturedStatusAsync(int ProfileID, bool Active)
        {
            using var connection = new SqliteConnection(_ConnectionString);
            await connection.OpenAsync();
            using var command = new SqliteCommand("UPDATE Profiles SET FeaturedAuthor = @Active WHERE id = @ProfileID", connection);
            command.Parameters.AddWithValue("@Active", Active);
            command.Parameters.AddWithValue("@ProfileID", ProfileID);
            await command.ExecuteNonQueryAsync();
        }

        // Check if a profile exists by ID
        public static async Task<bool> ProfileExistsByIdAsync(DirectoryContext _context, int ProfileId)
        {
            return await _context.Profiles.AnyAsync(p => p.Id == ProfileId);
        }

        // Check if a profile exists by email
        public static async Task<int> ProfileExistsByEmailAsync(DirectoryContext _context, string ProfileEmail)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == ProfileEmail);
            return profile?.Id ?? 0;
        }

        // Delete a profile by ID
        public static async Task DeleteProfileAsync(int ProfileID)
        {
            using var connection = new SqliteConnection(_ConnectionString);
            await connection.OpenAsync();
            using var command = new SqliteCommand("DELETE from Profiles WHERE id = @ProfileID", connection);
            command.Parameters.AddWithValue("@ProfileID", ProfileID);
            await command.ExecuteNonQueryAsync();
        }

        // Delete a deal by deal number and site ID
        public static async Task DeleteDealAsync(int DealNum, int SiteID)
        {
            using var connection = new SqliteConnection(_ConnectionString);
            await connection.OpenAsync();

            string query = DealNum switch
            {
                1 => "UPDATE Settings SET DealImage1 = NULL, DealURL1 = NULL WHERE ID = @SiteID",
                2 => "UPDATE Settings SET DealImage2 = NULL, DealURL2 = NULL WHERE ID = @SiteID",
                3 => "UPDATE Settings SET DealImage3 = NULL, DealURL3 = NULL WHERE ID = @SiteID",
                _ => throw new ArgumentException("Invalid DealNum", nameof(DealNum))
            };

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@SiteID", SiteID);
            await command.ExecuteNonQueryAsync();
        }
    }
}