
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace Blog.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string from, string to, string subject, string html)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Site Administration", from));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };

                //_configuration.Get<SmtpHiddenInfo>();
                SmtpHiddenInfo smtpHiddenInfo = new SmtpHiddenInfo();
                _configuration.GetSection("SmtpHiddenInfo").Bind(smtpHiddenInfo);

                // send email
                // ниже не путать с System.Net.Mail !!!
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(
                    smtpHiddenInfo.Host,
                    smtpHiddenInfo.Port,
                    (SecureSocketOptions)smtpHiddenInfo.SecureSocketOptions);

                // Possibly Exeption O_o !!!
                await smtp.AuthenticateAsync(
                    smtpHiddenInfo.User,
                    smtpHiddenInfo.Password);

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
