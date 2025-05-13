using Microsoft.AspNetCore.Mvc;
using ZRoomBackendApi.Services;
using ZRoomLoginLibrary.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRoomBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AuthService _authService;

        public LoginController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var userResponse = _authService.Login(loginDTO);

            if (userResponse == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(userResponse);
        }
    }
}
