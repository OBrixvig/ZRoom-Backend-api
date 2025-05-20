using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRoomUiTests
{
    [TestClass]
    public class LoginPageSeleniumUiTest
    {
        private IWebDriver driver;
        private readonly string url = "http://127.0.0.1:5500/index.html";

        [TestInitialize]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public void Cleanup() 
        {
            driver.Quit();
            driver.Dispose();
        }

        [TestMethod]
        public void LoginEmailInput_Exists_ShouldPass()
        {
            // Arrange
            var emailInput = driver.FindElement(By.Id("email"));
            var expectedResult = "bruger@edu.zealand.dk";


            // Act
            var actualResult = emailInput.GetAttribute("placeholder");

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void LoginPasswordInput_Exists_ShouldPass()
        {
            // Arrange
            var passwordInput = driver.FindElement(By.Id("password"));
            var expectedResult = "password";

            // Act
            var actualResult = passwordInput.GetAttribute("type");

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void LoginButton_Exist_ShouldPass()
        {
            // Arrange
            var submitButton = driver.FindElement(By.TagName("button"));
            var expectedResult = "btn btn-primary w-100";


            // Act
            var actualResult = submitButton.GetAttribute("class");


            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
