using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{
    public class AvailableBookingRepository
    {
        private readonly string _connectionString;

        public AvailableBookingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<AvailableBooking> GetAll()
        {
            List<AvailableBooking> bookings = new List<AvailableBooking>();
            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();

                string sql = "SELECT RoomId, Date, StartTime, EndTime FROM AvailableBookings";

                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TimeOnly startTime = TimeOnly.Parse(reader.GetTimeSpan(2).ToString());
                    TimeOnly endTime = TimeOnly.Parse(reader.GetTimeSpan(3).ToString());
                    AvailableBooking b = new AvailableBooking(
                        reader.GetString(0), reader.GetDateTime(1).Date,
                        startTime, endTime);

                    bookings.Add(b);
                }
                return bookings;
            }
        }

        public void DeleteAllOldBookings()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "DELETE FROM AvailableBookings " +
                             "WHERE DATEADD(SECOND, DATEDIFF(SECOND, 0, EndTime), CAST(Date AS DATETIME)) < GETDATE()";

                SqlCommand command = new SqlCommand(sql, connection);

                command.ExecuteNonQuery();
            }
        }
    }
}
