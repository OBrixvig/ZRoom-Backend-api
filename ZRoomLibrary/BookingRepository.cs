using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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

                string sqlQuery = "SELECT Id, RoomId, Date, UserEmail, Member1, Member2, Member3, StartTime, EndTime, PinCode FROM Booking WHERE UserEmail = @Email AND IsActive = 1";

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

        public async Task<Booking?> DeleteBooking(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Henter booking detaljer for at returnere og genskabe AvailableBookings
                        string selectQuery = @"
                                             SELECT Id, RoomId, Date, UserEmail, Member1, Member2, Member3, StartTime, EndTime, PinCode
                                             FROM Booking 
                                             WHERE Id = @Id";
                        Booking? booking = null;
                        using (SqlCommand selectCommand = new SqlCommand(selectQuery, conn, transaction))
                        {
                            selectCommand.Parameters.AddWithValue("@Id", id);
                            using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    TimeOnly startTime = TimeOnly.Parse(reader.GetTimeSpan(7).ToString());
                                    TimeOnly endTime = TimeOnly.Parse(reader.GetTimeSpan(8).ToString());
                                    booking = new Booking(
                                        reader.GetInt32(0),
                                        reader.GetString(1),
                                        reader.GetDateTime(2).Date,
                                        reader.GetString(3),
                                        reader.IsDBNull(4) ? null : reader.GetString(4),
                                        reader.IsDBNull(5) ? null : reader.GetString(5),
                                        reader.IsDBNull(6) ? null : reader.GetString(6),
                                        startTime,
                                        endTime,
                                        reader.GetString(9)
                                    );
                                }
                            }
                        }

                        if (booking == null)
                        {
                            return null;
                        }

                        // Opdater IsActive til false
                        string updateQuery = "UPDATE Booking SET IsActive = @IsActive WHERE Id = @Id";
                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@Id", id);
                            updateCommand.Parameters.AddWithValue("@IsActive", false);
                            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                            if (rowsAffected == 0)
                            {
                                return null;
                            }
                        }

                        // Sender booking informationer tilbage til AvailableBookings
                        string insertAvailableQuery = @"
                                                       INSERT INTO AvailableBookings (RoomId, Date, StartTime, EndTime)
                                                       VALUES (@RoomId, @Date, @StartTime, @EndTime)";
                        using (SqlCommand insertCommand = new SqlCommand(insertAvailableQuery, conn, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@RoomId", booking.Roomid);
                            insertCommand.Parameters.AddWithValue("@Date", booking.Date.Date);
                            insertCommand.Parameters.AddWithValue("@StartTime", booking.StartTime.ToTimeSpan());
                            insertCommand.Parameters.AddWithValue("@EndTime", booking.EndTime.ToTimeSpan());
                            int rows = await insertCommand.ExecuteNonQueryAsync();
                            if (rows == 0)
                            {
                                throw new Exception("Fejl: Kunne ikke indsætte i AvailableBookings.");
                            }
                        }

                        transaction.Commit();
                        return booking;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return null;
                    }
                }
            }
        }

       
        public async Task UpdateActiveBookings(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sqlQuery = @"UPDATE Booking
                                    SET IsActive = 0
                                    WHERE @UserEmail = UserEmail 
                                    AND DATEADD(SECOND, DATEDIFF(SECOND, 0, EndTime), CAST(Date AS DATETIME)) < GETDATE()";

                SqlCommand sqlCommand = new SqlCommand(sqlQuery, conn);

                sqlCommand.Parameters.AddWithValue("@UserEmail", email);

                await sqlCommand.ExecuteNonQueryAsync();
            }
        }
    }
}

