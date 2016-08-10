
#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;
using cfacore.shared.domain.user;
using cfares.domain.user;
using cfares.site.modules.user;
using Saml2SecurityToken = Microsoft.IdentityModel.Tokens.Saml2.Saml2SecurityToken;
using SecurityTokenHandler = Microsoft.IdentityModel.Tokens.SecurityTokenHandler;
using SecurityTokenHandlerCollection = Microsoft.IdentityModel.Tokens.SecurityTokenHandlerCollection;

#endregion

namespace cfares.site.modules.com.Security
{
	public static class SecurityTokenExtensions
	{
		private static readonly string[] _relevantClaimKeys = {
			"Username", "FirstName", "LastName", "EmailAddress", "AuthorityUID", "Role", "StoreIDs" };

		public static ResUser ToResUser( this List<Claim> claims)
		{
		    Dictionary<string, string> c = claims.GetRelevantClaimKeyValuePairs();
            string uid=c.GetClaimValue("AuthorityUID");
		    
            ResUser u = new ResUser(){UID = uid};


			//u.Username = c.GetClaimValue( "Username" );
			u.AuthorityUID = c.GetClaimValue( "AuthorityUID" );
            
            u.Authority = "atCfa";
            u.Email = c.GetClaimValue("EmailAddress");
		    if (string.IsNullOrEmpty(u.Email))
		    {
		        u.Email = ResUser.GenerateEmail();
		    }
		    u.Username = c.GetClaimValue("Username");
			u.Name = new Name( string.Format( "{0} {1}", 
				c.GetClaimValue( "FirstName" ), c.GetClaimValue( "LastName" ) ) );
		    var role = c.GetClaimValue("Role").ToLower().Replace("_audience","");
            
		    switch (role.ToLower())
		    {
                case "staff":
                case "contractor":
                    u.OperationRole=UserOperationRole.Admin;
		            break;
                case "operator":
                    u.OperationRole=UserOperationRole.Operator;
		            break;
                case "team":
                default:
                    u.OperationRole=UserOperationRole.Guide;
		            break;
		    }

			return u;
		}

		private static Dictionary<string, string> GetRelevantClaimKeyValuePairs( this List<Claim> claims  )
		{
            return claims.Where( o => _relevantClaimKeys.Contains( o.ClaimType ) )
                .ToDictionary( k => k.ClaimType, v => v.Value );
		}

	    public static string[] ToStoreIdArray( this List<Claim> claims )
	    {
		    Dictionary<string, string> c = claims.GetRelevantClaimKeyValuePairs();
			string storeIds = c.GetClaimValue("StoreIDs");
	        if (string.IsNullOrEmpty(storeIds.Trim())) storeIds = "00000";
	        return storeIds.Split(',').Select(x=>int.Parse(x.Trim()).ToString("D5")).ToArray();
	    }

	    private static string GetClaimValue( this Dictionary<string, string> c, string key )
        {
			return c.Where( o => o.Key == key ).Select( o => o.Value.Trim() ).FirstOrDefault() ?? string.Empty;
        }

		public static IEnumerable<Claim> GetClaimsFromSaml20Response( this string saml20Response )
		{
			IEnumerable<Claim> claims = null;
			saml20Response = saml20Response.Replace( "<saml2:Audience/>", "<saml2:Audience>http://res.site</saml2:Audience>" );
			using( var xReader = XmlReader.Create( new StringReader( saml20Response ) ) )
			{
				xReader.ReadToFollowing( "X509Certificate" );
				string certAsString = xReader.ReadElementString();
				byte[] bytes = new byte[certAsString.Length * sizeof(char)];
				Buffer.BlockCopy(certAsString.ToCharArray(), 0, bytes, 0, bytes.Length);

				SecureString pass = new SecureString();
				foreach (char c in "password") pass.AppendChar(c);
				X509Certificate2 embeddedCertificate = new X509Certificate2( bytes,
					pass, X509KeyStorageFlags.Exportable );

				//SecurityTokenHandlerCollection tokenhandlers = 
				//	SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();

				//SecurityTokenHandler vtokenhandler = ( from h in tokenhandlers
				//	where h.GetType() == typeof(Microsoft.IdentityModel.Tokens.Saml2.Saml2SecurityTokenHandler)
				//	select h ).First();

				SecurityTokenHandlerCollection tokenhandlers = new SecurityTokenHandlerCollection( 
					new SecurityTokenHandler[] {
						new ReservationSaml2SecurityTokenHandler(), 
					} );

				SecurityTokenHandler vtokenhandler = ( from h in tokenhandlers
					where h.GetType() == typeof(ReservationSaml2SecurityTokenHandler)
					select h ).First();

				vtokenhandler.Configuration.AudienceRestriction.AudienceMode = 
					System.IdentityModel.Selectors.AudienceUriMode.Never;

				vtokenhandler.Configuration.IssuerNameRegistry = new ReservationIssuerNameRegistry();

				xReader.ReadToFollowing("Assertion", "urn:oasis:names:tc:SAML:2.0:assertion");
				XmlReader t = xReader.ReadSubtree();
				SecurityToken vtoken = vtokenhandler.ReadToken( t );

				// Certificate when received does not contain a private key, so we have to compare the
				// .. received certificate to our server installed certificate to ensure validity
				// .. uses a server installed certificate identified by thumbprint used below

				// Thumbprints can be obtained from cert details in local certificate stores ( windows mmc, for ex )
				// SEANTOUCH - Generated and includes private key
				//const string thumbprint = "‎36 53 be d2 ae 94 d1 93 da 77 15 56 c4 d0 36 1d 81 81 62 d6";
				// CFA - When testing, currently no private key available so utlizing match verification
				const string thumbprint = "‎85 d9 f3 40 08 d3 07 f6 2c 52 e0 ac 78 7f 03 cd 73 ac 02 c4";

				X509Certificate2 validationCertificate = thumbprint.GetCertificate();

				bool matchingCertificates = validationCertificate.RawData
					.SequenceEqual( embeddedCertificate.RawData );

				if( ! matchingCertificates )
					throw new SecurityTokenValidationException( "Untrusted issuer token" );

				Saml2SecurityToken newToken = "res".GetSamlAssertionSignedWithCertificate( 
					vtoken, embeddedCertificate );

				ClaimsIdentityCollection claimsIdentityCollection = vtokenhandler.ValidateToken( newToken );
				IClaimsIdentity claimsIdentity = claimsIdentityCollection.FirstOrDefault();
				if( claimsIdentity != null ) claims = claimsIdentity.Claims;

				return claims;
			}
		}
	}
}
