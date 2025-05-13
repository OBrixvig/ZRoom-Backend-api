using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ZRoomLibrary
{
    public class AvailableBooking
    {
        public string RoomId { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public AvailableBooking(string roomid, DateTime date, TimeOnly starttime, TimeOnly endtime)
        {
            RoomId = roomid;
            Date = date;
            StartTime = starttime;
            EndTime = endtime;
        }
    }
}
