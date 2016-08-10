
#region Imports

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cfacore.shared.service.Locations;

#endregion

namespace cfacore.Tests
{
    [TestClass]
    public class LocationServiceTest
	{
	    #region Context / Test Attributes

	    public TestContext TestContext { get; set; }
	    
	    private LocationAPIService _locationAPIService;
		public LocationAPIService LocationAPIService
		{
			get
			{
				if( _locationAPIService == null ) _locationAPIService = new LocationAPIService();
				return _locationAPIService;
			}
		}

	    #region Additional Test Attributes
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

	    #endregion

        [TestMethod]
        public void GetByZipCodeValid()
        {
			List<LocationResult> r = LocationAPIService.GetByPostalCode( "30097", 25 ).ToList();
			Assert.IsTrue( r.Count > 0 );
        }

		
        [TestMethod]
        public void GetByZipCodeAndClientKeysValid()
        {
			// 00593 = John's Creek
			// 01208 = Sugarloaf Corporate Center

			List<LocationResult> r = LocationAPIService.GetByPostalCodeAndClientKeys( 
				"30097", 25, new List<string> { "00593", "01208" } ).ToList();

			Assert.IsTrue( r.Count > 0 );
        }

    }
}
