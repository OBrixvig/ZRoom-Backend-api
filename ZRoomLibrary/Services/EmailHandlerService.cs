using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace ZRoomLibrary.Services
{
    public class EmailHandlerService
    {
        private readonly string _apiKey;

        public EmailHandlerService(IConfiguration configuration)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("SendGrid API key is not configured.");
            }
        }

        public async Task SendVerificationCode(string toEmail, string PinCode, string RoomId, TimeOnly StartTime, TimeOnly EndTime, DateTime date)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("Jonasbuchner36@gmail.com", "Z-Room");
            var to = new EmailAddress(toEmail);
            var subject = "Booking Bekræftelse";
            var plainTextContent =
            $"Booking Bekræftelse\n" +
            $"----------------------\n" +
            $"Dato: {date.Date}\n" +
            $"Rum: {RoomId}\n" +
            $"Pinkode: {PinCode}\n" +
            $"Booking start: {StartTime}\n" +
            $"Slut tid for booking: {EndTime}\n\n" +
            $"Husk at tjekke ind mindst 5 minutter før booking start.\n\n" +
            $"Tak fordi du bruger Z-Room!";

            var htmlContent = $@"
             <div style='font-family: Arial, sans-serif; max-width: 500px; margin: auto; border: 1px solid #eee; border-radius: 8px; padding: 24px; background: #fafbfc;'>
             <h2 style='color: #2d7ff9;'>Booking Bekræftelse</h2>
             <p>
             <strong>Dato:</strong> {date}<br/>
             <strong>Rum:</strong> {RoomId}<br/>
             <strong>Pinkode:</strong> <span style='font-size: 1.5em; color: #2d7ff9; font-weight: bold;'>{PinCode}</span>
             </p>
             <p>
             <strong>Booking start:</strong> {StartTime}<br/>
             <strong>Slut tid for booking:</strong> {EndTime}
             </p>
             <p style='margin-top: 24px; color: #555;'>
             Husk at tjekke ind mindst 5 minutter før booking start.
             </p>
             <hr style='margin: 24px 0;'/>
             <p style='font-size: 0.9em; color: #888;'>Tak fordi du bruger Z-Room!</p>
             </div>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
