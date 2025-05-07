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

            string pinCode = GeneratePinCode();

            
            HttpContext.Session.SetString("VerificationCode", pinCode);

            var emailHandler = new EmailHandler();
            await emailHandler.SendVerificationCode(RecipientEmail, pinCode);

            Message = "Pinkode sendt til e-mail.";
            return Page(); 
        }

        private string GeneratePinCode()
        {
            var random = new Random();
            return random.Next(1000, 10000).ToString();
        }
    }
}
