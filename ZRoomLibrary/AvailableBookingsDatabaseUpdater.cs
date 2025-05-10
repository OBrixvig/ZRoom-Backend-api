using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{
    public class AvailableBookingsDatabaseUpdater
    {
        private readonly string _connectionString;

        public AvailableBookingsDatabaseUpdater(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task RotateBookingsAsync()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"
                   DELETE FROM AvailableBookings
                   WHERE Date < CAST(GETDATE() AS DATE);            

                   INSERT INTO AvailableBookings (RoomId, TimeSlot, Date, StartTime, EndTime)
                   SELECT
                       r.RoomId,
                       t.Slot,
                       DATEADD(DAY, 3, CAST(GETDATE() AS DATE)) AS Date,
                   	t.StartTime,
                   	t.EndTime
                   FROM
                       Rooms r
                   CROSS JOIN
                       TimeSlots t;
                ";


                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConnection);

                await sqlConnection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
