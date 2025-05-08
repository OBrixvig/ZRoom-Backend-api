using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ZRoomBackendApi.Pages
{
    public class EmailSenderModel : PageModel
    {
        [BindProperty]
        public string RecipientEmail { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(RecipientEmail))
            {
                Message = "E-mailen må ikke være tom.";
                return Page();
            }

            string pinCode = await GenerateAndStorePinCodeAsync(RecipientEmail);

            
            HttpContext.Session.SetString("VerificationCode", pinCode);

            var emailHandler = new EmailHandler();
            await emailHandler.SendVerificationCode(RecipientEmail, pinCode);

            Message = "Pinkode sendt til e-mail.";
            return Page(); 
        }

        private async Task<bool> ValidatePinCodeAsync(string email, string enteredCode)
        {
            using (var connection = new SqlConnection("YourConnectionString"))
            {
                string query = "SELECT COUNT(*) FROM PinCodes WHERE Email = @Email AND Code = @Code";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Code", enteredCode);

                await connection.OpenAsync();
                int resultCount = (int)await command.ExecuteScalarAsync();
                int count = resultCount;

                return count > 0;
            }
        }

        private async Task<string> GenerateAndStorePinCodeAsync(string email)
        {
            var random = new Random();
            string pinCode = random.Next(1000, 10000).ToString();

            
            using (var connection = new SqlConnection("YourConnectionString"))
            {
                string query = "INSERT INTO PinCodes (Email, Code, CreatedAt) VALUES (@Email, @Code, @CreatedAt)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Code", pinCode);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            return pinCode;
        }
    }
}
