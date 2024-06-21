using System.Net;
using System.Net.Mail;

namespace BDFA.BL
{
    public class Manager
    {
        private static string? _SMTPFrom;
        private static string? _SMTPUserName;
        private static string? _SMTPPassword;
        private static string?  _SMTPHost;
        private static int _SMTPPort;
        private static bool _SMTPSSL;

        public static void InitializeSMTPSettings(IConfiguration configuration)
        {
            _SMTPFrom = configuration["SMTPSettings:SMTPFrom"];
            _SMTPUserName = configuration["SMTPSettings:SMTPUserName"];
            _SMTPPassword = configuration["SMTPSettings:SMTPPassword"];
            _SMTPHost = configuration["SMTPSettings:SMTPHost"];
            _SMTPPort = Convert.ToInt32(configuration["SMTPSettings:SMTPPort"]);
            _SMTPSSL = Convert.ToBoolean(configuration["SMTPSettings:SMTPSSL"]);
        }

        public static void SendMail(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_SMTPHost) || _SMTPPort == 0)
            {
                throw new InvalidOperationException("SMTP settings are not initialized.");
            }

            SmtpClient client = new SmtpClient(_SMTPHost, _SMTPPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_SMTPUserName, _SMTPPassword),
                EnableSsl = _SMTPSSL
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_SMTPFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            client.Send(mailMessage);
        }
    }
}
