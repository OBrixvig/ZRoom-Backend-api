using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomLibrary
{
    public class RoomRepository
    {
        private readonly string _connectionString;
        
        public RoomRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Room> GetAll()
        { 
            var roomList = new List<Room>();

            using (SqlConnection connection= new SqlConnection(_connectionString))
            { 
                connection.Open();

                string sqlQuery = "SELECT RoomId, PictureLink FROM Rooms";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Room newRoom = new(reader.GetString(0), reader.GetString(1));
                    roomList.Add(newRoom);
                }
            }

            return roomList;
        }

    }
}
