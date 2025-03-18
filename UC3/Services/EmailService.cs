using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace UC3.Services
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            // Lees de configuratie uit appsettings.json
            var emailSettings = configuration.GetSection("EmailSettings");
            _smtpHost = emailSettings["Host"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(emailSettings["Port"] ?? "587");
            _smtpUsername = emailSettings["Mail"];
            _smtpPassword = emailSettings["Password"];
            _senderEmail = emailSettings["Mail"];
            _senderName = emailSettings["DisplayName"] ?? "WorkoutLogger App";
        }

        public async Task SendVerificationCodeAsync(string toEmail, int verificationCode)
        {
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress(_senderName, _senderEmail));
            mail.To.Add(MailboxAddress.Parse(toEmail));

            mail.Subject = "Je verificatiecode voor WorkoutLogger";

            var body = new TextPart("html")
            {
                Text = $@"
                    <h1>Verificatiecode</h1>
                    <p>Gebruik de volgende code om in te loggen op WorkoutLogger:</p>
                    <h2 style='background-color: #f0f0f0; padding: 10px; text-align: center;'>{verificationCode}</h2>
                    <p>Deze code is 10 minuten geldig.</p>
                    <p>Als je geen toegang hebt aangevraagd, kun je deze e-mail negeren.</p>
                "
            };

            mail.Body = body;

            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync(
                    _smtpHost,
                    _smtpPort,
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await smtp.SendAsync(mail);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Loggen fout ofd oorgeven
                throw new Exception($"Fout bij het verzenden van e-mail: {ex.Message}", ex);
            }
        }
    }
}