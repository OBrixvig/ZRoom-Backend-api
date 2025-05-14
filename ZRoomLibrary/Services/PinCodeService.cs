using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ZRoomLibrary.Services
{
    public class PinCodeService
    {
        private readonly string _connectionString;

        public PinCodeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("loginDB")
                ?? throw new InvalidOperationException("Connection string 'loginDB' is not configured.");
        }

        public async Task<string> GenerateAndStorePinCodeAsync(string email)
        {
            var random = new Random();
            string pinCode = random.Next(1000, 10000).ToString();

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Bookings (Email, Code, CreatedAt) VALUES (@Email, @Code, @CreatedAt)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Code", pinCode);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            return pinCode;
        }

        public async Task<bool> ValidatePinCodeAsync(string email, string enteredCode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM PinCodes WHERE Email = @Email AND Code = @Code";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Code", enteredCode);

                await connection.OpenAsync();
                int count = (await command.ExecuteScalarAsync() as int?) ?? 0;

                return count > 0;
            }
        }
    }
}
