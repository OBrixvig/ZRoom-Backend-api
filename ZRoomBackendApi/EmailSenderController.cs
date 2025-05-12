using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ZRoomBackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailSenderController : ControllerBase
    {
        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] string recipientEmail)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                return BadRequest("E-mailen må ikke være tom.");
            }

            string pinCode = await GenerateAndStorePinCodeAsync(recipientEmail);

            HttpContext.Session.SetString("VerificationCode", pinCode);

            var emailHandler = new EmailHandler();
            await emailHandler.SendVerificationCode(recipientEmail, pinCode);

            return Ok("Pinkode sendt til e-mail.");
        }

        [HttpPost("validate-code")]
        public async Task<IActionResult> ValidateCode([FromBody] PinCodeValidationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.EnteredCode))
            {
                return BadRequest("E-mail og pinkode må ikke være tomme.");
            }

            bool isValid = await ValidatePinCodeAsync(request.Email, request.EnteredCode);

            if (isValid)
            {
                return Ok("Check-in succesfuldt! Du har nu adgang til rummet.");
            }
            else
            {
                return BadRequest("Pinkoden er forkert. Prøv igen.");
            }
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
                int count = (int)await command.ExecuteScalarAsync();

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

    public class PinCodeValidationRequest
    {
        public string Email { get; set; }
        public string EnteredCode { get; set; }
    }
}
