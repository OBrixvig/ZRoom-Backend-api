using System;
using System.Threading.Tasks;
using ZRoomBackendApi;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter your email address:");
        // string email = Console.ReadLine()?.Trim();
        string email = "jonasbuchner36@gmail.com";

        if (string.IsNullOrWhiteSpace(email.Trim()))
        {
            Console.WriteLine("Email cannot be empty.");
            return;
        }

        var emailHandler = new EmailHandler();
        string testCode = "1234"; // Use a static code for testing purposes

        try
        {
            // Directly send the email without involving any database logic
            await emailHandler.SendVerificationCode(email, testCode);
            Console.WriteLine($"Verification code sent to {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}
