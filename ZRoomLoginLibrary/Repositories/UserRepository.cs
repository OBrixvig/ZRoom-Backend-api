using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZRoomLoginLibrary.Models;

namespace ZRoomLoginLibrary.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=ZRoomLoginService;Integrated Security=True;Encrypt=False";

        public List<User> GetAll()
        {
            List<User> outputList = new();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sqlQuery = "SELECT Email, Name, StudentId, ClassId FROM Users";

                SqlCommand command = new(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    outputList.Add(new User(reader.GetString(0), reader.GetString(1),
                                            reader.GetInt32(2), reader.GetInt32(3)));
                }

                return outputList;
            }
        }

    }
}
