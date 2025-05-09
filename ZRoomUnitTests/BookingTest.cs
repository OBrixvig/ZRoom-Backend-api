using Microsoft.Identity.Client;
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

            string actualroom = b.Roomid;

            Assert.AreEqual(expectedroom, actualroom);
        }
        [TestMethod]
        public void TestGetAllMethod()
        {
            //BookingRepository bookingrepo = new BookingRepository();
            //int expectedresult = 128;

            //int actualresult = bookingrepo.GetAll().Count();

            //Assert.AreEqual(expectedresult, actualresult);
        }
        [TestMethod]
        public void CreateB()
        {
            //BookingRepository abr = new BookingRepository();

            //AvailableBookingRepository ab = new AvailableBookingRepository();

            //Booking a = new("DD1", "10:00-12:00", new DateTime(2025, 05, 08).Date, "penis@edu.zealand.dk");

            //int countbefore = ab.GetAll().Count();

            //abr.CreateBooking(a);

            //int expectedresult = ab.GetAll().Count();



            //Assert.AreNotEqual(countbefore, expectedresult);
        }
    }
}
