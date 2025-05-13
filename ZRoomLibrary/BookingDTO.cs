using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{

    public record BookingDTO(string Roomid, 
        string Timeslot, DateTime date, string userEmail);
}
