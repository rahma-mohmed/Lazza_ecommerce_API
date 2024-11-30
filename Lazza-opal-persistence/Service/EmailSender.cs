namespace Lazza.opal.persistence.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;

        public EmailSender(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage("engrahma1422003@gmail.com", email, subject, message)
            {
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
