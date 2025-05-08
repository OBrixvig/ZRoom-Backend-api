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
        private string _roomid;
        private string _timeslot;
        private DateTime _date;

        public string RoomId { get; set; }
        public string TimeSlot { get; set; }
        public DateTime Date { get; set; }

        public AvailableBooking(string roomid, string timeslot, DateTime date)
        {
            RoomId = roomid;
            TimeSlot = timeslot;
            Date = date;
        }
    }
}
