using cfacore.site.controllers._event;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cfares.domain._event;
using cfares.domain._event._ticket.tours;

namespace cfares.Tests
{
    
    
    /// <summary>
    ///This is a test class for TicketServiceTest and is intended
    ///to contain all TicketServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TicketServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest()
        {
            TicketService target = new TicketService(); 
            string ID = "1";
            Ticket expected = new Ticket();
            expected = target.Load("1");
            Ticket actual;
            actual = target.Load(ID);

            // Testing that expected,actual are equivalent.
            Assert.AreEqual(expected.ToChecksum(), actual.ToChecksum());

            // Testing that notExpected,actual are not equivalent.
            Ticket notExpected;
            notExpected = target.Load("2");
            Assert.AreNotEqual(notExpected.ToChecksum(), actual.ToChecksum());

        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTourCapacityRestrictionTest()
        {
            TicketService target = new TicketService();
            string ID = "461";
            
            TourTicket testTicket;
            testTicket = target.LoadTour(ID);


            testTicket.GuestCount += 100;
            try
            {
                //target.CapacityCheck(target, new cfacore.shared.domain._base.DomainServiceEventArgs { target = testTicket, isNew = false });
                //Assert.Fail();
            }catch(Exception ex){
                Console.Write(ex.Message);
            }

            testTicket.GuestCount -= 101;
            try
            {
                target.CapacityCheck(target, new cfacore.shared.domain._base.DomainServiceEventArgs { target = testTicket, isNew = false });                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Assert.Fail();                
            }

        }

        /// <summary>
        ///A test for LoadTour
        ///</summary>
        [TestMethod()]
        public void LoadTourTest()
        {
            TicketService target = new TicketService(); 
            string ID = "1"; 
            TourTicket expected = new TourTicket();
            expected = target.LoadTour("1");
            TourTicket actual;
            actual = target.LoadTour(ID);
            Assert.AreEqual(expected.ToChecksum(), actual.ToChecksum());

            // Assert that LoadTourTest() loads instance of TourTicket
            TourTicket test = target.LoadTour("5");
            Assert.IsInstanceOfType(test, typeof(TourTicket));

            // Seeing if LoadTourTest() outcome is also recognized as Ticket type.
            Assert.IsInstanceOfType(test, typeof(Ticket));
        }
    }
}
