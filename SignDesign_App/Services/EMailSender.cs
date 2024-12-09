//using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;

namespace SignDesign_App.Services
{
    public class EMailSender //: IEmailSender
    {
        public IConfiguration _config;

        public EMailSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage, params string[] attachments)
        {
            try
            {
                // Get SMTP settings from appsettings.json configuration file.
                string fromName = _config.GetSection("smtpsettings").GetSection("fromName").Value;
                string fromEmailAddress = _config.GetSection("smtpsettings").GetSection("fromEmailAddr").Value;
                string password = _config.GetSection("smtpsettings").GetSection("password").Value;
                int port = Convert.ToInt32(_config.GetSection("smtpsettings").GetSection("port").Value);
                string smtpServer = _config.GetSection("smtpsettings").GetSection("smtpserver").Value;
                bool useSSL = Convert.ToBoolean(_config.GetSection("smtpsettings").GetSection("usessl").Value);
                string[] emails = email.Split(';');

                // Compose email message.
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmailAddress));
                //message.To.Add(new MailboxAddress(email, email));
                message.Subject = subject;

                foreach (string emailAddress in emails)
                {
                    message.To.Add(new MailboxAddress(emailAddress, emailAddress));
                }

                // Add HTML and/or plain text content.
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = htmlMessage; // HTML formatted content.
                //bodyBuilder.TextBody = ""; // Plain text content. Use HtmlBody instead.

                // Add attachments.
                foreach (string attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(attachment);
                }

                // Set message body.
                message.Body = bodyBuilder.ToMessageBody();

                // Connect to SMTP server and send email.
                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect(smtpServer, port, useSSL);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(fromEmailAddress, password);

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }

            return Task.CompletedTask;
        }
    }
}
