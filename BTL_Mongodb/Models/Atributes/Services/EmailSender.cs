
using System.Net;
using System.Net.Mail;

namespace BTL_Mongodb.Models.Atributes.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("hoquan06012004@gmail.com", "bfetohtynlhaswlz")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("hoquan06012004@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true // 🟢 THIẾT LẬP HTML
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }

    }
}
