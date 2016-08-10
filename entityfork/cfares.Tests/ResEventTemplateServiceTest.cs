using cfacore.site.controllers._event;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cfares.domain._event;

namespace cfares.Tests
{
    
    
    /// <summary>
    ///This is a test class for ResEventTemplateServiceTest and is intended
    ///to contain all ResEventTemplateServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ResEventTemplateServiceTest
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
            ResEventTemplateService target = new ResEventTemplateService();
            string ID = "1";
            ResTemplate expected = new ResTemplate();
            expected = target.Load("1");
            ResTemplate actual;
            actual = target.Load(ID);
            
            // Testing that expected,actual are equal.
            Assert.AreEqual(expected.ToChecksum(), actual.ToChecksum());
        }
    }
}
