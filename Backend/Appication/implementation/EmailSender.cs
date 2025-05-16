using Application.abstracts;
using Application.Helper;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
namespace Application.Implementation
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSetting _emailSettings;

        public EmailSender(EmailSetting emailSettings)
        {
            _emailSettings = emailSettings;
        }
        public async Task<string> SendEmail(string email, string Message, string? reason)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(_emailSettings.FromEmail, _emailSettings.Password);
                    var bodybuilder = new BodyBuilder
                    {
                        HtmlBody = Message,
                        TextBody = "wellcome",
                    };
                    var message = new MimeMessage
                    {
                        Body = bodybuilder.ToMessageBody()
                    };

                    message.From.Add(new MailboxAddress("Future Team", _emailSettings.FromEmail));
                    message.To.Add(new MailboxAddress("testing", email));
                    message.Subject = reason == null ? "No Submitted" : reason;
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return "Failed";
            }
        }
    }
}
