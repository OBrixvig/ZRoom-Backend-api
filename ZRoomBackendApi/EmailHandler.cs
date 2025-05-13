using SendGrid;
using SendGrid.Helpers.Mail;
using System;

namespace ZRoomBackendApi
{
    public class EmailHandler
    {
        private readonly string _apiKey;

        public EmailHandler()
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("SendGrid API key is not configured in the environment variables.");
            }
            Console.WriteLine($"Retrieved API Key: {_apiKey}");
        }

        public async Task SendVerificationCode(string toEmail, string code)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("Jonasbuchner36@gmail.com", "Z-Room");
            var to = new EmailAddress(toEmail);
            var subject = "Din bekræftelseskode For Booking";
            var plainTextContent = $"Din pinkode er: {code}";
            var htmlContent = $"<p>Din pinkode er:</p><h2>{code}</h2>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            Console.WriteLine($"SendGrid Response Status Code: {response.StatusCode}");
            Console.WriteLine($"SendGrid Response Body: {await response.Body.ReadAsStringAsync()}");

            if ((int)response.StatusCode >= 400)
            {
                throw new Exception($"Failed to send email. Status code: {response.StatusCode}, Body: {await response.Body.ReadAsStringAsync()}");
            }
        }
    }
}
