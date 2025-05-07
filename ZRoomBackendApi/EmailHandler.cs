using SendGrid;
using SendGrid.Helpers.Mail;
namespace ZRoomBackendApi
{
    public class EmailHandler
    {
        private readonly string apiKey = "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";

        public async Task SendVerificationCode(string toEmail, string code)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("Jonasbuchner36@gmail.com", "Z-Room");
            var to = new EmailAddress(toEmail);
            var subject = "Din bekræftelseskode For Booking";
            var plainTextContent = $"Din pinkode er: {code}";
            var htmlContent = $"<p>Din pinkode er:</p><h2>{code}</h2>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}
