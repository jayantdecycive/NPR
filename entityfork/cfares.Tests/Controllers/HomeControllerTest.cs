using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cfaresv2.Areas.MyAccount.Controllers;
using cfaresv2.Controllers;

namespace cfares.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            LogOnController controller = new LogOnController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("Welcome to ASP.NET MVC!", result.ViewBag.Message);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            LogOnController controller = new LogOnController();

            // Act
			//ViewResult result = controller.About() as ViewResult;

            // Assert
			//Assert.IsNotNull(result);
        }
    }
}
