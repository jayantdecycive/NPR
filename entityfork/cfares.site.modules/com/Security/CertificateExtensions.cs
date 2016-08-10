
#region Imports

using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Saml2Assertion = Microsoft.IdentityModel.Tokens.Saml2.Saml2Assertion;
using Saml2SecurityToken = Microsoft.IdentityModel.Tokens.Saml2.Saml2SecurityToken;

#endregion

namespace cfares.site.modules.com.Security
{
	public static class CertificateExtensions
	{
		public static X509Certificate2 GetCertificate( this string certificateThumbprint )
		{
			certificateThumbprint = certificateThumbprint.Replace( " ", string.Empty ).ToUpper();

			X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			store.Open(OpenFlags.ReadOnly);

			X509Certificate2 c = null;
			foreach( X509Certificate2 mCert in store.Certificates )
				if( certificateThumbprint.Equals( mCert.Thumbprint, 
					StringComparison.InvariantCultureIgnoreCase ) ) { c = mCert; break; }

			if( c == null ) throw new SecurityTokenValidationException(
				string.Format( "Unable to locate required X509 certificate [ Thumbprint = {0}, Certificate Store = {1} ]", certificateThumbprint, "LocalMachine"  ) );

			return c;
		}

		public static Saml2SecurityToken GetSamlAssertionSignedWithCertificate( this string nameIdentifierClaim, 
			SecurityToken unSignedToken, string certificateThumbprint )
		{
			X509Certificate2 c = GetCertificate( certificateThumbprint );
			return GetSamlAssertionSignedWithCertificate( nameIdentifierClaim, unSignedToken, c );
		}

		public static Saml2SecurityToken GetSamlAssertionSignedWithCertificate( this string nameIdentifierClaim, 
			SecurityToken unSignedToken, X509Certificate2 certificate )
		{
			Saml2SecurityToken ust2 = (Saml2SecurityToken) unSignedToken;
			Saml2Assertion assertion = ust2.Assertion;

			// Currently validing using match verification, as compared to X509SigningCredentials,
			// .. since private key is required to generate X509credentials

			//X509SigningCredentials clientSigningCredentials = new X509SigningCredentials( certificate );
			//assertion.SigningCredentials = clientSigningCredentials;
			//X509SecurityToken xsecurityToken = new X509SecurityToken( certificate );
			//return new Saml2SecurityToken( assertion, unSignedToken.SecurityKeys, xsecurityToken );

			return new Saml2SecurityToken( assertion, unSignedToken.SecurityKeys, unSignedToken );
		}
	}
}
