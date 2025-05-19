using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using ZRoomLoginLibrary.Repositories;

namespace ZRoomBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepo)
        {
              _userRepository = userRepo;  
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("String is empty.");
            }

            var users = _userRepository.GetByEmailOrName(query);
          
            return Ok(users);
        }
    }
}
