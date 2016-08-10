using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security.Tokens;

using cfacore.wse.aloha.dao.com.alohaenterprise.memberlink;



namespace cfacore.wse.aloha.dao._base
{
	public class AlohaService
	{
#if DEBUG
        public bool Production = false;
#else
        public bool Production = true;
#endif

        public const String wrapUserID = "w620953";
        public const String wrapPassword = "%306743561S"; // Contact your Radiant Systems representative for this WS UserID
        
        public const String companyID_staging = "cfa01";// - staging
        public const String password_staging = "mychickfila7"; // - staging

        public const String companyID = "cfa05";
        public const String password = "mychickfila"; // Contact your Radiant Systems representative for this WS Password

        public const String userID = "webservice"; // Contact your Radiant Systems representative for this WS UserID
        public MemberLinkWS mlws;
        
#if DEBUG
        public AlohaService(bool production = false){
#else
        public AlohaService(bool production = true){
#endif
            
            /*TODO:MANAGE THIS*/
            //production = !production;
            mlws = new MemberLinkWS();
            Production = production;
            
            UsernameToken token = new UsernameToken(wrapUserID, wrapPassword, PasswordOption.SendPlainText);

	        //throw new NotSupportedException();
			// SH - Disabling after required .NET 4.5 upgrade / compilation issue
			mlws.SetClientCredential(token);
			mlws.SetPolicy("ClientPolicy");

        }

        public void SetRequestCredential(Request request)
        {
#if DEBUG
            if (Production == false)
            {
                request.companyID = companyID_staging;
                request.password = password_staging;
            }
            else
            {
                request.companyID = companyID;
                request.password = password;
            }
#else
            request.companyID = companyID;
            request.password = password;
#endif

            request.userID = userID;
            request.requestTime = DateTime.Now;
        } 
	}
    public class AlohaExecution
    {
        int _status;
        string _message;
        public const int SUCCESS_CODE = 0;

        public bool Success
        {
            get { return _status == SUCCESS_CODE; }
        }

        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public AlohaExecution(int status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        public AlohaExecution(cfacore.wse.aloha.dao.com.alohaenterprise.memberlink.Response resp)
        {
            this.Status = resp.executionStatus;
            this.Message = resp.executionStatusDescription;
        }
    }
    public class LoginResult {

        private MemberProfile profile = null;
        public MemberProfile Profile {
            get { return profile; }
            set { profile = value; }
        }
        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public bool Success{
            get {
                return profile != null && profile.profileExists;
            }
            set { 
                //void
            }
        }

        public LoginResult() { 
        
        }

    }
}
