using System.Net;
using System.Net.Mail;

namespace BDFA
{
    public class Manager
    {
        public static void SendEmail(string toEmailAddress, string subject, string emailBody)
        {
            try
            {
                // Configure the SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.forwardemail.net")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("support@smtp.ari-integration.com", "3c529c1c57ad8a192c879be5"),
                    EnableSsl = true,
                };

                // Create the email message
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("admin@buydirectfromauthors.com"),
                    Subject = subject,
                    Body = emailBody,
                    IsBodyHtml = true, // Set to false if the body is plain text
                };

                mailMessage.To.Add(toEmailAddress);

                // Send the email
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email. Error: {ex.Message}");
            }
        }
    }
}
