
#region Imports

using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Configuration;
using System.Web;

using System.Web.Mvc;
using System.IO;

using System.Configuration;

#endregion

namespace cfacore.shared.service.email
{
    public abstract class EmailService
	{
		#region Properties

		public SmtpClient Client = null;
	    public string DefaultFrom = string.Empty;
        protected string AdminEmail = string.Empty;
        protected string DebugEmail = string.Empty;

	    private string _baseUrl;
	    public virtual string BaseUrl
	    {
		    get {
			    return _baseUrl ?? ( _baseUrl = string.Format( "{0}://{1}",
					HttpContext.Current.Request.Url.Scheme,
					HttpContext.Current.Request.Headers["host"] ) );
		    }
		    protected set { _baseUrl = value; }
	    }

	    public virtual string ThemeUrl { get; protected set; }

		#endregion

		#region Constructor

		public EmailService()
        {
            DefaultFrom = ConfigurationManager.AppSettings["default-from-email"];
            AdminEmail = ConfigurationManager.AppSettings["admin-email"];
            DebugEmail = ConfigurationManager.AppSettings["debug-email"];

            Client = new SmtpClient {UseDefaultCredentials = false};

            var settings = (SmtpSection)System.Web.Configuration.WebConfigurationManager.GetSection("system.net/mailSettings/smtp");

            Client.Credentials = new System.Net.NetworkCredential(
                settings.Network.UserName,
                settings.Network.Password);

            Client.EnableSsl = true;
        }

		#endregion

		#region Email Testing

		public bool Test(string recipient, string subject)
        {            
            string body = ViewToString<string>("TestEmail", null);
            return Message(recipient, "[cfares Test Email] " + subject, body);
        }

        public bool Test(string recipient) {
            string subject = string.Format("{0} - {1}",DateTime.Now.ToLongDateString(),DateTime.Now.ToLongTimeString());
            return Test(recipient, subject);
        }

		#endregion

		#region Message Composition

		public bool Message(string recipient, string subject, string body) {
            return Message(new[] { recipient }, subject, body, null, new ViewDataDictionary(), new TempDataDictionary(), null, null);
        }

        public bool Message(string[] recipients, string subject, string body)
        {
            return Message(recipients, subject, body, null, new ViewDataDictionary(), new TempDataDictionary(), null, null);
        }

        public bool Message(string recipient, string subject, string body, Attachment[] attachments)
        {
            return Message(new[] { recipient }, subject, body, attachments, new ViewDataDictionary(), new TempDataDictionary(), null, null);
        }

        public bool Message(string[] recipients, string subject, string body, Attachment[] attachments)
        {
            return Message(recipients, subject, body, attachments, new ViewDataDictionary(), new TempDataDictionary(), null, null);
        }

        public bool Message(string recipient, string subject, string body, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            return Message(new[] { recipient }, subject, body, null, viewData, tempData, null, null);
        }

        public bool Message(string[] recipients, string subject, string body, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            return Message(recipients, subject, body, null, viewData, tempData, null, null);
        }

        public bool Message(string recipient, string subject, string body, Attachment[] attachments, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            return Message(new[] { recipient }, subject, body, attachments, viewData, tempData, null,null);
        }

        public bool Message(string[] recipients, string subject,  string body, Attachment[] attachments, ViewDataDictionary viewData, TempDataDictionary tempData, string[] bcc, string[] cc)
        {            
            MailMessage message = new MailMessage
	            {
		            From = new MailAddress(DefaultFrom),
		            Subject = subject,
		            IsBodyHtml = true,
		            Body = body
	            };

            if (recipients!=null)
	        foreach (string recipient in recipients.ToList().Distinct())
            {
                message.To.Add(new MailAddress(recipient));
            }
            if (cc != null)
            foreach (string recipient in cc.ToList().Distinct())
            {
                message.CC.Add(new MailAddress(recipient));
            }
            if (bcc != null)
            foreach (string recipient in bcc.ToList().Distinct())
            {
                message.Bcc.Add(new MailAddress(recipient));
            }

            if (attachments != null)
            {
                foreach (Attachment attachment in attachments.ToList().Distinct())
                    message.Attachments.Add(attachment);
            }

			try
            {
                Client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }

		#endregion

		#region Template Helpers

		public string ViewToString<T>(string viewName, object model, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
	        using (new StringWriter())
	        {
		        string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Views/Shared/Email/" + viewName + ".cshtml");
		        if (string.IsNullOrEmpty(filePath))
		        {
			        filePath = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["mvc-template-root"]) + viewName + ".cshtml";
			        if (string.IsNullOrEmpty(filePath))
				        throw new Exception("MVC Templates could not be found.");
		        }
                
		        string renderedText = "";
		        if (File.Exists(filePath))
		        {
					string cshtmlText = File.ReadAllText(filePath);
			        try
			        {
				        renderedText = RenderText( cshtmlText, (T) model );
			        }
			        catch( Exception ex )
			        {
				        /*StringBuilder sb = new StringBuilder();
				        foreach( CompilerError e in ex.Errors )
					        sb.AppendFormat( "[ {0} ]", e );
                        */
						throw new ApplicationException( "Template compilation error(s) " + ex.Message );
			        }
                    //renderedText = RenderText(cshtmlText, (T)model);
		        }

		        return renderedText;
	        }
        }

		public abstract string RenderText<T>( string cshtmlText, T model );

		//public abstract EmailContextModel GetContextModel( object model );

	    public string ViewToString<T>(string viewName, object model)
        {
            return ViewToString<T>(viewName, model, new ViewDataDictionary(), new TempDataDictionary());
        }

		#endregion
	}
}
