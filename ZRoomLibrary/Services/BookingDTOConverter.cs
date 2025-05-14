using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary.Services
{
    public static class BookingDTOConverter
    {
        public static Booking ToBooking(this BookingDto dto)
        {
            return new Booking(
                dto.Roomid,
                dto.Date,
                dto.UserEmail,
                dto.Member1,
                dto.Member2,
                dto.Member3,
                dto.StartTime,
                dto.EndTime,
                dto.PinCode
            );
        }
    }
}
