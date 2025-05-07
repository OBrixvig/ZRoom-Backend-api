using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZRoomLibrary;

namespace ZRoomUnitTests
{
    [TestClass]
    public class BookingTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            string expectedroom = "Dkdd";

            Booking b = new("Dkdd", "202", new DateTime(2025, 05, 07).Date, "ddld");

            string actualroom = b.Room;

            Assert.AreEqual(expectedroom, actualroom);
        }

        [TestMethod]
        public void TestGetAllMethod()
        {
            BookingRepository bookingrepo = new BookingRepository();
            int expectedresult = 128;

            int actualresult = bookingrepo.GetAll().Count();

            Assert.AreEqual(expectedresult, actualresult);
        }
    }
}
