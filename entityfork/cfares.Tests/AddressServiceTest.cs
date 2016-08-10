using cfacore.service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cfacore.shared.domain.user;
using System.Collections.Generic;

namespace cfares.Tests
{
    
    
    /// <summary>
    ///This is a test class for AddressServiceTest and is intended
    ///to contain all AddressServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddressServiceTest
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
            AddressService target = new AddressService();
            string ID = "1";
            Address expected = new Address();
            
            expected = target.Load("1");
            Address actual;
            actual = target.Load(ID);
            
            // Testing that Address objects expected,actual are equivalent.
            Assert.AreEqual(expected.ToChecksum(), actual.ToChecksum());

            // Testing that Address objects notExpected,actual are not equal.
            Address notExpected = target.Load("2");
            Assert.AreNotEqual(notExpected, actual);

            // Testing that Load returns Address object
            Address test = target.Load("5");
            Assert.IsInstanceOfType(test, typeof(Address));
        }
    }
}
