using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{
    public class Booking
    {
        public string Roomid { get; set; }
        public string TimeSlot { get; set; }
        public DateTime Date { get; set; }
        public string UserEmail { get; set; }
        public string? Member1 { get; set; }
        public string? Member2 { get; set; }
        public string? Member3 { get; set; }
        public Booking(string roomid, string timeslot, DateTime date, string useremail, string? member1, string? member2, string? member3)
        {
            Roomid = roomid;
            TimeSlot = timeslot;
            Date = date;
            UserEmail = useremail;
            Member1 = member1;
            Member2 = member2;
            Member3 = member3;
        }
    }
}
