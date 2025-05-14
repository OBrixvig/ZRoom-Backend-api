using Microsoft.AspNetCore.Mvc;
using ZRoomLibrary;
using ZRoomLibrary.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRoomBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AvailableBookingRepository _availableBookingRepository;
        private readonly BookingRepository _bookingRepository;
        public BookingController(BookingRepository bookingRepository, AvailableBookingRepository availableBookingRepository)
        {
            _bookingRepository = bookingRepository;
            _availableBookingRepository = availableBookingRepository;
        }

        // GET: api/<BookingController>
        [HttpGet]
        public IActionResult Get()
        {
            List<AvailableBooking> list = _availableBookingRepository.GetAll();

            if(list.Count == 0)
            {
                return NoContent();
            }
            else
            {
                return Ok(list);
            }
        }

        [HttpGet]
        [Route("{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // GET: api/<BookingController>/test@edu.zealand.dk
        public IActionResult GetByEmail(string email)
        {
            var list = _bookingRepository.GetByEmail(email);

            if (list == null)
            {
                return NotFound();
            }

            return Ok(list);
        }

        // POST api/<BookingController>
        [HttpPost]
        public IActionResult Post([FromBody]BookingDto value)
        {
            var booking = BookingDTOConverter.ToBooking(value);
            var bookingToCreate = _bookingRepository.CreateBooking(booking);

            if (bookingToCreate != null)
            {
                return Created("api/" + bookingToCreate.Roomid, bookingToCreate);
            }
            else
            {
                return BadRequest();
            }
        }

        // PUT api/<BookingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<BookingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
