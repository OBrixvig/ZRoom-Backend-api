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

        public string GenerateAndStorePinCodeAsync()
        {
            var random = new Random();
            string pinCode = random.Next(1000, 10000).ToString();

            return pinCode;
        }
    }
}
