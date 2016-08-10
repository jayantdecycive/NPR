using cfacore.site.controllers._event;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cfares.domain._event;

namespace cfares.Tests
{
    
    
    /// <summary>
    ///This is a test class for OccurrenceServiceTest and is intended
    ///to contain all OccurrenceServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OccurrenceServiceTest
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
            OccurrenceService target = new OccurrenceService(); 
            string ID = "1"; 
            Occurrence expected = new Occurrence();
            expected = target.Load("1");
            Occurrence actual;
            actual = target.Load(ID);
            
            // Testing that actual,expected are equal.
            Assert.AreEqual(expected.ToChecksum(), actual.ToChecksum());

            // Testing that actual,notExpected are not equal.
            Occurrence notExpected;
            notExpected = target.Load("2");
            Assert.AreNotEqual(notExpected, actual);

            // Using Assert.Fail
            string badId = null;
            Occurrence badOccurrence = new Occurrence();
            try
            {
                badOccurrence = target.Load(badId);
                Assert.Fail("This is a test.");
            }
            catch
            {
            }
        }

    }
}
