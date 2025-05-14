using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZRoomLibrary.Models;
using ZRoomLibrary.Services;

//skal måske ikke bruges 
namespace ZRoomBackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailSenderController : ControllerBase
    {
        private readonly EmailHandlerService _emailHandler;
        private readonly PinCodeService _pinCodeService;

        public EmailSenderController(EmailHandlerService emailHandler, PinCodeService pinCodeService)
        {
            _emailHandler = emailHandler;
            _pinCodeService = pinCodeService;
        }

        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] string recipientEmail, string roomId, TimeOnly startTime, TimeOnly endTime )
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                return BadRequest("E-mailen må ikke være tom.");
            }

            string pinCode =  _pinCodeService.GenerateAndStorePinCodeAsync(recipientEmail);
            
            //await _emailHandler.SendVerificationCode(recipientEmail, pinCode, roomId, startTime, endTime);

            return Ok("Pinkode sendt til e-mail.");
        }

        [HttpPost("validate-code")]
        public async Task<IActionResult> ValidateCode([FromBody] PinCodeValidationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.EnteredCode))
            {
                return BadRequest("E-mail og pinkode må ikke være tomme.");
            }

            bool isValid = await _pinCodeService.ValidatePinCodeAsync(request.Email, request.EnteredCode);

            if (isValid)
            {
                return Ok("Check-in succesfuldt! Du har nu adgang til rummet.");
            }
            else
            {
                return BadRequest("Pinkoden er forkert. Prøv igen.");
            }
        }
    }
}
