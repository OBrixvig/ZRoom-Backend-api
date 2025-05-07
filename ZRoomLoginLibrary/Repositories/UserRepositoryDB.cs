using Microsoft.Data.SqlClient;
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
        private readonly string _connectionString = "Server=localhost;Database=ZRoomLoginService;Integrated Security=True;Encrypt=False";

        private record _user (string Email, string Password);
        private readonly List<_user> _users;

        public UserRepositoryDB()
        {
            _users = new List<_user>() 
            { 
                new _user("kaj007@edu.zealand.dk", "password"),
                new _user("pp@edu.zealand.dk", "pp")
            };

        }

        public User Authenticate(LoginDTO loginCredentials)
        {
            var user = _users.FirstOrDefault(x => x.Email == loginCredentials.Email);

            if (user == null || user.Password != loginCredentials.Password)
            {
                return null;
            }

            return new User(user.Email, "succes", 1, 1);
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
