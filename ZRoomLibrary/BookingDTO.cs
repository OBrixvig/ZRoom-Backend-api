using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{

    public record BookingDto(
        string Roomid,
        DateTime Date,
        string UserEmail,
        string? Member1,
        string? Member2,
        string? Member3,
        TimeOnly StartTime,
        TimeOnly EndTime
        
    );
}
