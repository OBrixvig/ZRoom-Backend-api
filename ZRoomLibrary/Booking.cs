using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{
    public class Booking
    {
        public int Id {  get; set; }
        public string Roomid { get; set; }
        public DateTime Date { get; set; }
        public string UserEmail { get; set; }
        public string? Member1 { get; set; }
        public string? Member2 { get; set; }
        public string? Member3 { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string PinCode { get; set; }

        public Booking(string roomid, DateTime date, string useremail, string? member1, string? member2, string? member3, TimeOnly startTime, TimeOnly endTime, string pincode)
        {
            Roomid = roomid;
            Date = date;
            UserEmail = useremail;
            Member1 = member1;
            Member2 = member2;
            Member3 = member3;
            StartTime = startTime;
            EndTime = endTime;
            PinCode = pincode;
        }

        public Booking(int id, string roomid, DateTime date, string userEmail, string? member1, string? member2, string? member3, TimeOnly startTime, TimeOnly endTime, string pincode) : 
        this(roomid, date, userEmail, member1, member2, member3, startTime, endTime, pincode)
        {
            Id = id;
        }
    }
}
