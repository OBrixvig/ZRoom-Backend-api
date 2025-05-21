using Microsoft.AspNetCore.Mvc;
using ZRoomLibrary;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRoomBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly RoomRepository _repo;

        public RoomsController(RoomRepository repo)
        {
            _repo = repo;
        }

        // GET: api/<RoomsController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get()
        {
            var roomList = _repo.GetAll();

            try
            {
                return roomList.Count <= 0 ? NoContent() : Ok(roomList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
