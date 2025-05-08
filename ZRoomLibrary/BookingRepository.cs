using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{
    public class BookingRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=Bookings;Integrated Security=True;Encrypt=False";

        public void CreateBooking(Booking booking)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Start a transaction
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // First query: INSERT into Bookings
                    string insertQuery = @"
                    INSERT INTO Booking (RoomId, TimeSlot, Date, UserEmail, IsActive)
                    VALUES (@RoomId, @TimeSlot, @Date, @UserEmail, @IsActive)";

                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, conn, transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@RoomId", booking.Roomid);
                        insertCommand.Parameters.AddWithValue("@TimeSlot", booking.TimeSlot);
                        insertCommand.Parameters.AddWithValue("@Date", booking.Date.Date);
                        insertCommand.Parameters.AddWithValue("@UserEmail", booking.UserEmail);
                        insertCommand.Parameters.AddWithValue("@IsActive", 1);

                        insertCommand.ExecuteNonQuery();
                    }

                    // Second query: DELETE from AvailableBookings
                    string deleteQuery = @"
                    DELETE FROM AvailableBookings
                    WHERE RoomId = @RoomId AND TimeSlot = @TimeSlot AND Date = @Date";
                    int deletesuccesfull = 0; 
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, conn, transaction))
                    {
                        
                        deleteCommand.Parameters.AddWithValue("@RoomId", booking.Roomid);
                        deleteCommand.Parameters.AddWithValue("@TimeSlot", booking.TimeSlot);
                        deleteCommand.Parameters.AddWithValue("@Date", booking.Date.Date);

                        
                        deletesuccesfull = deleteCommand.ExecuteNonQuery();
                    }

                    // Commit transaction if both succeed
                    if(deletesuccesfull >= 1)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        throw new Exception();
                    }
                        Console.WriteLine("Booking successful and available slot removed.");
                }
                catch (Exception ex)
                {
                    // Rollback transaction if either fails
                    transaction.Rollback();
                    Console.WriteLine("Transaction failed: " + ex.Message);
                }
            }
        }
    }
}
