﻿using Microsoft.Data.SqlClient;
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
        private readonly string _connectionString;

        public BookingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Booking? CreateBooking(Booking booking)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Start a transaction
                SqlTransaction transaction = conn.BeginTransaction();
                    string insertQuery = @"
                INSERT INTO Booking (RoomId, TimeSlot, Date, UserEmail, IsActive, Member1, Member2, Member3)
                VALUES (@RoomId, @TimeSlot, @Date, @UserEmail, @IsActive, @Member1, @Member2, @Member3)";


                using (SqlCommand insertCommand = new SqlCommand(insertQuery, conn, transaction))
                {
                    insertCommand.Parameters.AddWithValue("@RoomId", booking.Roomid);
                    insertCommand.Parameters.AddWithValue("@TimeSlot", booking.TimeSlot);
                    insertCommand.Parameters.AddWithValue("@Date", booking.Date.Date);
                    insertCommand.Parameters.AddWithValue("@UserEmail", booking.UserEmail);
                    insertCommand.Parameters.AddWithValue("@IsActive", 1);
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
                    return booking;
                }

                return null;
                }
            }
        }
    }

