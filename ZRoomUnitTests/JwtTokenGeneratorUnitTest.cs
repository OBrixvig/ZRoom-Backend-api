using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZRoomBackendApi.Services;
using ZRoomLoginLibrary.Models;

namespace ZRoomUnitTests
{
    [TestClass]
    public class JwtTokenGeneratorUnitTest
    {
        private IConfiguration _configuration;
        private JwtTokenGenerator _tokenGenerator;

        [TestInitialize]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                 { "Jwt:Key", "PisskrissFordTrucksChineseFoodMiaKhadufa420ppBombardinoCrocadillo" },
                { "Jwt:Issuer", "PpLoginService" },
                { "Jwt:Audience", "ZRoomBooking" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenGenerator = new JwtTokenGenerator(_configuration);
        }

        [TestMethod]
        public void GenerateToken_ShouldContainExpectedClaims()
        {
            // Arrange
            var testUser = new User("svendtest@edu.zealand.dk", "sventest", 50, 99);


            // Act
            var tokenString = _tokenGenerator.GenerateToken(testUser);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);


            // Assert
            Assert.AreEqual("PpLoginService", token.Issuer);
            Assert.AreEqual("ZRoomBooking", token.Audiences.First());

            Assert.IsTrue(token.Claims.Any(c => c.Type == "name" && c.Value == "sventest"));
            Assert.IsTrue(token.Claims.Any(c => c.Type == "studentId" && c.Value == "50"));
            Assert.IsTrue(token.Claims.Any(c => c.Type == "classId" && c.Value == "99"));
            Assert.IsTrue(token.Claims.Any(c => c.Type == "email" && c.Value == "svendtest@edu.zealand.dk"));
        }
    }
}
