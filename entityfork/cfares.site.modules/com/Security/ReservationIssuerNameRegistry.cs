
#region Imports

using System;
using System.IdentityModel.Tokens;

#endregion

namespace cfares.site.modules.com.Security
{
	public class ReservationIssuerNameRegistry : Microsoft.IdentityModel.Tokens.IssuerNameRegistry
	{
		// called by X509SecurityTokenHandler.Validate
		public override string GetIssuerName(SecurityToken securityToken)
		{
			// Currently validing using match verification since private key 
			// .. is required to generate X509credentials

			// ValidateIssuerToken( securityToken );
			
			X509SecurityToken x509Token = (X509SecurityToken) securityToken;
			return x509Token.Certificate.FriendlyName;
		}

		// called by Saml11SecurityTokenHandler.Validate and Saml2SecurityTokenHandler.Validate
		public override string GetIssuerName(SecurityToken securityToken, string requestedIssuerName)
		{
			// Currently validing using match verification since private key
			// .. is required to generate X509credentials
			
			// ValidateIssuerToken( securityToken );
			
			return requestedIssuerName;
		}

		public override string GetWindowsIssuerName()
		{
			return "WINDOWS AUTHORITY";
		}

		public static void ValidateIssuerToken( SecurityToken securityToken )
		{
			X509SecurityToken t = securityToken as X509SecurityToken;
			if (t == null) throw new SecurityTokenValidationException("Invalid token [ Security token type ]");

			if (t.ValidFrom > DateTime.Now || DateTime.Now > t.ValidTo) 
				throw new SecurityTokenValidationException("Invalid token [ Validity dates ]");
		}
	}
}
