using cfacore.site.controllers.shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cfacore.shared.domain.store;

namespace cfares.Tests
{
    
    
    /// <summary>
    ///This is a test class for StoreServiceTest and is intended
    ///to contain all StoreServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StoreServiceTest
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
            StoreService target = new StoreService(); 
            string ID = "1"; 
            Store expected = new Store();
            
            expected = target.Load("1");
            Store actual;
            actual = target.Load(ID);

            // Test that expected,actual are equivalent.
            Assert.AreEqual(expected.ToChecksum(), actual.ToChecksum());

            // Test that notExpected,actual are not equivalent.
            Store notExpected = target.Load("2");
            Assert.AreNotEqual(notExpected, actual);

            // Test that Load() gives Store object
            Assert.IsInstanceOfType(actual, typeof(Store));
        }
    }
}
