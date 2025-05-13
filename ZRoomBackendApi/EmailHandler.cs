using SendGrid;
using SendGrid.Helpers.Mail;

namespace ZRoomBackendApi
{
    public class EmailHandler
    {
        private readonly string apiKey = "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\r\n".Trim();

        public async Task SendVerificationCode(string toEmail, string code)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("Jonasbuchner36@gmail.com", "Z-Room");
            var to = new EmailAddress(toEmail);
            var subject = "Din bekræftelseskode For Booking";
            var plainTextContent = $"Din pinkode er: {code}";
            var htmlContent = $"<p>Din pinkode er:</p><h2>{code}</h2>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            // Log the response status code
            Console.WriteLine($"SendGrid Response Status Code: {response.StatusCode}");

            if ((int)response.StatusCode >= 400)
            {
                throw new Exception($"Failed to send email. Status code: {response.StatusCode}, Body: {await response.Body.ReadAsStringAsync()}");
            }
        }

    }
}
