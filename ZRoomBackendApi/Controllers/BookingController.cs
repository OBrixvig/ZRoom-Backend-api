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
        private readonly PinCodeService _pinCodeService;
        private readonly EmailHandlerService _emailHandlerService;
        public BookingController(BookingRepository bookingRepository, AvailableBookingRepository availableBookingRepository, PinCodeService pinCodeService, EmailHandlerService emailHandler)
        {
            _bookingRepository = bookingRepository;
            _availableBookingRepository = availableBookingRepository;
            _pinCodeService = pinCodeService;
            _emailHandlerService = emailHandler;
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

        // Updated POST method to handle the issue with 'roomId' property access
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BookingDto value)
        {
            string pinCode = _pinCodeService.GenerateAndStorePinCodeAsync();
            var booking = BookingDTOConverter.ToBooking(value, pinCode);
            var bookingToCreate = await _bookingRepository.CreateBooking(booking);


            if (bookingToCreate != null)
            {
                await _emailHandlerService.SendVerificationCode(booking.UserEmail, booking.PinCode, booking.Roomid, booking.StartTime, booking.EndTime, booking.Date);

                if (booking.Member1 != null)
                {
                    await _emailHandlerService.SendVerificationCode(booking.Member1, booking.PinCode, booking.Roomid, booking.StartTime, booking.EndTime, booking.Date);
                }
                if (booking.Member2 != null)
                {
                    await _emailHandlerService.SendVerificationCode(booking.Member2, booking.PinCode, booking.Roomid, booking.StartTime, booking.EndTime, booking.Date);
                }
                if (booking.Member3 != null)
                {
                    await _emailHandlerService.SendVerificationCode(booking.Member3, booking.PinCode, booking.Roomid, booking.StartTime, booking.EndTime, booking.Date);
                }

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _bookingRepository.DeleteBooking(id);

            if (booking != null)
            {
                // indsæt email for brugeren.

                if (booking.Member1 != null)
                {
                    //indsæt for member 1. og så videre. Mail er ikke lavet endnu.
                }

                return Ok(); 
            }

            return NotFound();
        }
    }
}
