
using cfares.site.modules.mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cfares.Tests
{
	[TestClass]
	public class NprEmailServiceTest
	{
		[TestMethod]
		public void SendPostageAppMessageTest()
		{
			NprEmailService s = new NprEmailService( true );
			const string postageAppClientKey = "exbx7F9irhYQdrRQkp2hySSdk1pSsDxN";
			const string postageAppBaseUri = "https://api.postageapp.com/v.1.0/send_message.json";
			const string htmlEmailContent = "html email content <b>bold text here</b>";
			const string emailAddress = "sean@decycive.com";
			const string subject = "Test email";

			s.DefaultFrom = "noreply@decycive.com";

			//s.SendPostageAppMessage( postageAppClientKey, postageAppBaseUri, 
				//htmlEmailContent, "", emailAddress,  subject );
		}
	}
}
