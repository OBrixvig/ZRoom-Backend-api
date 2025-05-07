using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{
    public class BookingRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=Bookings;Integrated Security=True;Encrypt=False";

        public List<Booking> GetAll()
        {
            List<Booking> bookings = new List<Booking>();
            using(SqlConnection connection = new(_connectionString))
            {
                connection.Open();

                string sql = "SELECT RumId, TimeSlot, BookingDate FROM AvailableBookings";

                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Booking b = new Booking(reader.GetInt32(0), reader.GetString(1), reader.GetDateTime(2));

                    bookings.Add(b);
                }
                return bookings;
            }
        }
    }
}
