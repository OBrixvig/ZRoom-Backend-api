using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZRoomLoginLibrary.Repositories;

namespace ZRoomUnitTests
{
    [TestClass]
    public class LoginRepositoryUserTests
    {
        [TestMethod]
        public void GetAll_TestConnection_ShouldPass()
        {
            // Arrange
            UserRepository testRepo = new UserRepository();

            // Act
            var actualResult = testRepo.GetAll().Count;

            // Assert
            Assert.IsTrue((actualResult != 0));
        }
    }
}
