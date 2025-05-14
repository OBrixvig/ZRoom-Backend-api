using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ZRoomLibrary.Services;
using static SendGrid.BaseClient;

namespace ZRoomLibrary
{
    public class BookingRepository
    {
        private readonly string _connectionString;

        private readonly EmailHandlerService _emailHandler;

        public BookingRepository(string connectionString, EmailHandlerService emailHandler)
        {
            _connectionString = connectionString;
            _emailHandler = emailHandler;
        }

        public List<Booking>? GetByEmail(string email)
        {
            List<Booking> bookings = new List<Booking>();

            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT Id, RoomId, Date, UserEmail, Member1, Member2, Member3, StartTime, EndTime, PinCode FROM Booking WHERE UserEmail = @Email";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Email", email);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TimeOnly startTime = TimeOnly.Parse(reader.GetTimeSpan(7).ToString());
                    TimeOnly endTime = TimeOnly.Parse(reader.GetTimeSpan(8).ToString());

                    string? member1 = reader.IsDBNull(4) ? null : reader.GetString(4);
                    string? member2 = reader.IsDBNull(5) ? null : reader.GetString(5);
                    string? member3 = reader.IsDBNull(6) ? null : reader.GetString(6);

                    Booking b = new(reader.GetInt32(0), reader.GetString(1),
                                    reader.GetDateTime(2).Date, reader.GetString(3),
                                    member1, member2,
                                    member3, startTime,
                                    endTime, reader.GetString(9));

                    bookings.Add(b);
                }

                if (bookings.Count == 0)
                {
                    return null;
                }

                return bookings;
            }
            ;

        }

        public async Task<Booking?> CreateBooking(Booking booking)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Start a transaction
                SqlTransaction transaction = conn.BeginTransaction();
                string insertQuery = @"
                INSERT INTO Booking (RoomId, Date, UserEmail, IsActive, Member1, Member2, Member3, StartTime, EndTime, PinCode)
                VALUES (@RoomId,  @Date, @UserEmail, @IsActive, @Member1, @Member2, @Member3, @StartTime, @EndTime, @PinCode)";


                using (SqlCommand insertCommand = new SqlCommand(insertQuery, conn, transaction))
                {
                    insertCommand.Parameters.AddWithValue("@RoomId", booking.Roomid);
                    insertCommand.Parameters.AddWithValue("@Date", booking.Date.Date);
                    insertCommand.Parameters.AddWithValue("@UserEmail", booking.UserEmail);
                    insertCommand.Parameters.AddWithValue("@IsActive", 1);
                    insertCommand.Parameters.AddWithValue("@StartTime", booking.StartTime);
                    insertCommand.Parameters.AddWithValue("@EndTime", booking.EndTime);
                    insertCommand.Parameters.AddWithValue("@PinCode", booking.PinCode);

                    if (booking.Member1 != null)
                    {
                        insertCommand.Parameters.AddWithValue("@Member1", booking.Member1);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@Member1", DBNull.Value);
                    }
                    if (booking.Member2 != null)
                    {
                        insertCommand.Parameters.AddWithValue("@Member2", booking.Member2);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@Member2", DBNull.Value);
                    }
                    if (booking.Member3 != null)
                    {
                        insertCommand.Parameters.AddWithValue("@Member3", booking.Member3);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@Member3", DBNull.Value);
                    }

                    insertCommand.ExecuteNonQuery();

                    await _emailHandler.SendVerificationCode(booking.UserEmail, booking.PinCode, booking.Roomid, booking.StartTime, booking.EndTime, booking.Date);

                    if (booking.Member1 != null)
                    {
                        await _emailHandler.SendVerificationCode(booking.Member1, booking.PinCode, booking.Roomid, booking.StartTime, booking.EndTime, booking.Date);
                    }
                    if (booking.Member2 != null)
                    {
                        await _emailHandler.SendVerificationCode(booking.Member2, booking.PinCode, booking.Roomid, booking.StartTime, booking.EndTime, booking.Date);
                    }
                    if (booking.Member3 != null)
                    {
                        await _emailHandler.SendVerificationCode(booking.Member3, booking.PinCode, booking.Roomid, booking.StartTime, booking.EndTime, booking.Date);
                    }
                }
                // Second query: DELETE from AvailableBookings
                string deleteQuery = @"
                DELETE FROM AvailableBookings
                WHERE RoomId = @RoomId AND Date = @Date AND StartTime = @StartTime AND EndTime = @EndTime";
                int deletesuccesfull = 0;
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, conn, transaction))
                {

                    deleteCommand.Parameters.AddWithValue("@RoomId", booking.Roomid);
                    deleteCommand.Parameters.AddWithValue("@Date", booking.Date.Date);
                    deleteCommand.Parameters.AddWithValue("@StartTime", booking.StartTime);
                    deleteCommand.Parameters.AddWithValue("@EndTime", booking.EndTime);


                    deletesuccesfull = deleteCommand.ExecuteNonQuery();
                }

                // Commit transaction if both succeed
                if (deletesuccesfull >= 1)
                {
                    transaction.Commit();
                    return booking;
                }

                return null;
            }
        }

        public async Task<Booking> DeleteBooking(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Hent booking detaljer for at genskabe AvailableBookings
                string selectQuery = @"
            SELECT RoomId, Date, StartTime, EndTime, UserEmail, Member1, Member2, Member3
            FROM Booking 
            WHERE Id = @Id";

                Booking? booking = null;
                using (SqlCommand selectCommand = new SqlCommand(selectQuery, conn))
                {
                    selectCommand.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TimeOnly startTime = TimeOnly.Parse(reader.GetTimeSpan(2).ToString());
                            TimeOnly endTime = TimeOnly.Parse(reader.GetTimeSpan(3).ToString());

                            booking = new Booking(
                                id,
                                reader.GetString(0),
                                reader.GetDateTime(1).Date, 
                                reader.GetString(4), 
                                reader.IsDBNull(5) ? null : reader.GetString(5), 
                                reader.IsDBNull(6) ? null : reader.GetString(6),
                                reader.IsDBNull(7) ? null : reader.GetString(7), 
                                startTime,
                                endTime,
                                "" // Skal være pincode. men det fucker så den er bare tom.
                            );
                        }
                    }
                }



                if (booking == null)
                {
                    return false; 
                }

                // Slet booking
                string deleteQuery = "DELETE FROM Booking WHERE Id = @Id";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, conn))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return false; 
                    }
                }

                // Tilføj tilbage til AvailableBookings så den kan bookes igen.
                string insertAvailableQuery = @"
            INSERT INTO AvailableBookings (RoomId, Date, StartTime, EndTime)
            VALUES (@RoomId, @Date, @StartTime, @EndTime)";

                using (SqlCommand insertCommand = new SqlCommand(insertAvailableQuery, conn))
                {
                    insertCommand.Parameters.AddWithValue("@RoomId", booking.Roomid);
                    insertCommand.Parameters.AddWithValue("@Date", booking.Date.Date);
                    insertCommand.Parameters.AddWithValue("@StartTime", booking.StartTime);
                    insertCommand.Parameters.AddWithValue("@EndTime", booking.EndTime);

                    insertCommand.ExecuteNonQuery();
                }
                return true;
            }
        }
    }
}

