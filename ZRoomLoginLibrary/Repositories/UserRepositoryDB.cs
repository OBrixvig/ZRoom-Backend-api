using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZRoomLoginLibrary.Models;

namespace ZRoomLoginLibrary.Repositories
{
    public class UserRepositoryDB : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepositoryDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User? Authenticate(LoginDTO loginCredentials)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            { 
                connection.Open();

                string sqlQueryAuth = "SELECT Email, PasswordHash FROM Credentials WHERE Email = @email AND PasswordHash = @passwordHash";

                SqlCommand commandAuth = new(sqlQueryAuth, connection);

                commandAuth.Parameters.AddWithValue("@email", loginCredentials.Email);
                commandAuth.Parameters.AddWithValue("@passwordHash", loginCredentials.PasswordHash);

                var result = commandAuth.ExecuteScalar();

                if (result != null)
                {
                    string sqlQueryData = "SELECT Email, Name, StudentId, ClassId FROM Users WHERE Email = @email";

                    SqlCommand commandData = new(sqlQueryData, connection);

                    commandData.Parameters.AddWithValue("@email", loginCredentials.Email);

                    SqlDataReader reader = commandData.ExecuteReader();

                    while (reader.Read())
                    { 
                        return new User(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3));
                    }
                }
                return null;
            }
        }

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
