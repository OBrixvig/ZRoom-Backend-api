using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZRoomLoginLibrary.Models;

namespace ZRoomBackendApi.Services
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            // Claims indeholder data der vedrører useren som den generede token skal have med.
            var claims = new[]
            {
                new Claim("name", user.Name),
                new Claim("studentId", user.StudentId.ToString()),
                new Claim("classId", user.ClassId.ToString()),
                new Claim("email", user.Email)
            };

            // Laver en security key fra en hemmelig kode i appsettings.json
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            // Laver en signering af credentials med key og Hmac-Sha256 algoritmen
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Laver selve token med signering fra vores service, modtager betegnelse og de claims vi har lavet samt de signerede credentials.
            // Der tilføjes også en udløbstid på Token.
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );


            // Omskriver den genererede token til en string og returnerer den.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
