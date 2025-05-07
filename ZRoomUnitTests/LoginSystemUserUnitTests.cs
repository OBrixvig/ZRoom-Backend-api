using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZRoomLoginLibrary.Models;

namespace ZRoomUnitTests
{
    [TestClass]
    public class LoginSystemUserUnitTests
    {
        [TestMethod]
        public void Constructor_SetsPropertiesCorrect_ShouldPass()
        {
            // Arrange
            string email = "testmail@edu.zealand.dk";
            string name = "anders";
            int studyId = 1;
            int classId = 1;

            // Act 
            User testUser = new(email, name, studyId, classId);

            // Assert
            Assert.AreEqual(email, testUser.Email);
            Assert.AreEqual(name, testUser.Name);
            Assert.AreEqual(studyId, testUser.StudentId);
            Assert.AreEqual(classId, testUser.ClassId);
        }

        [TestMethod]
        public void ChangeValueOfProper_CorrectNewValue_ShouldPass()
        {
            // Arrange
            string newName = "Svend";
            User testUser = new("test@edu.zealand.dk", "Arne", 1, 1);
            string expectedResult = newName;

            // Act
            testUser.Name = newName;
            var actualResult = testUser.Name;


            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void InvalidEmail_ShouldThrowException()
        {
            // Arrange
            string wrongEmail = "test@hotmail.com";


            // Act
            Action expectedException = () => new User(wrongEmail, "gg", 1, 2);
            Debug.WriteLine(expectedException);

            // Assert
            Assert.ThrowsException<ArgumentException>(expectedException);
        }
    }
}
