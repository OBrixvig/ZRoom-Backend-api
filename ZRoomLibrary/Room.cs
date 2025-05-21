using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{
    public class Room
    {
        public string RoomId { get; set; }
        public string PictureLink { get; set; }

        public Room(string roomId, string pictureLink)
        {
            RoomId = roomId;
            PictureLink = pictureLink;
        }
    }
}
