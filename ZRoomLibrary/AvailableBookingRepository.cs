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
        private readonly string _connectionString = "Server=localhost;Database=Bookings;Integrated Security=True;Encrypt=False";

        public List<AvailableBooking> GetAll()
        {
            List<AvailableBooking> bookings = new List<AvailableBooking>();
            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();

                string sql = "SELECT RoomId, TimeSlot, Date FROM AvailableBookings";

                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    AvailableBooking b = new AvailableBooking(reader.GetString(0), reader.GetString(1), reader.GetDateTime(2));

                    bookings.Add(b);
                }
                return bookings;
            }
        }
    }
}
