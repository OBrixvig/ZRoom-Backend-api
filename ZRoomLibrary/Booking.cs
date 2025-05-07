using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{
    public class Booking
    {
        private int _roomId;
        private string _room;
        private string _timeslot;
        private DateTime _date;
        private string _useremail;
        private bool _isActive;
        private List<string>? _members;

        public int RoomId { get; set; }
        public string Room { get; set; }
        public string TimeSlot { get; set; }
        public DateTime Date { get; set; }
        public string UserEmail { get; set; }
        public bool IsActive { get; set; }
        public List<string>? Members { get; set; }

        public Booking()
        {
            
        }
        public Booking(int roomid, string timeslot, DateTime date)
        {
            RoomId = roomid;
            TimeSlot = timeslot;
            Date = date;
        }
        public Booking(string room, string timeslot, DateTime date, string useremail)
        {
            Room = room;
            TimeSlot = timeslot;
            Date = date;
            UserEmail = useremail;
        }
    }
}
