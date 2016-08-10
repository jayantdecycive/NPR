
#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSFederation;
using cfares.domain.store;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository.store;
using cfares.site.modules.Helpers;
using cfares.site.modules.com.application;
using cfares.site.modules.user;

#endregion

namespace cfares.site.modules.com.Security
{
	public static class SSO
	{
		#region SAML20Parameters

		public static class SAML20Parameters
		{
			public const string SAMLResponse = "SAMLResponse";
			public const string RelayState = "RelayState";
		}

		#endregion

		public static bool SSOAuthenticateResponseVerification()
		{
			string tokenXml = null;

			// Support for pure SAML 2.0 integration
			if( HttpContext.Current.Request[ SAML20Parameters.SAMLResponse ] != null )
			{
				try
				{
					tokenXml = HttpContext.Current.Request[ SAML20Parameters.SAMLResponse ];
					string d = tokenXml.Contains("%3D") ? HttpUtility.UrlDecode(tokenXml) : tokenXml;
					if( d != null ) tokenXml = Encoding.UTF8.GetString( Convert.FromBase64String( d ) );
				}
				catch( Exception ) {}
			}

			// Check for WS Federation response
			else if( HttpContext.Current.Request.Form[ WSFederationConstants.Parameters.Result ] != null )
			{
				// Process response from STS / IDP
				SignInResponseMessage m = WSFederationMessage.CreateFromNameValueCollection(
					WSFederationMessage.GetBaseUrl(HttpContext.Current.Request.Url.TranslatePort()),
					HttpContext.Current.Request.Form) as SignInResponseMessage;
				if( m == null ) 
					throw new ApplicationException( "Invalid SignInResponseMessage received" );

				tokenXml = m.Result;
			}

			// If response token received, validate, parse claims and sign-in user
			if( ! string.IsNullOrWhiteSpace( tokenXml ) )
			{
				List<Claim> claims = tokenXml.GetClaimsFromSaml20Response().ToList();
                ProcessClaim( claims.ToResUser(), claims.ToStoreIdArray() );
				return true;
			}

			return false;
		}

	    public static ResUser ProcessClaim(ResUser claimsUser, string[] storeIds)
	    {
            IResContext context = ReservationConfig.GetContext();
            UserMembershipRepository ur = new UserMembershipRepository(context);
            LocationRepository locationRepo = new LocationRepository(context);

            ResUser u = claimsUser;
		    ur.EnsureEmailDomainAppliedToUsername( u );
            u = ur.EnsureUserExists(u);
            
            if (storeIds.Any())
            {
                ur.EnsureUserStoreRelationship(u, storeIds);
                if (u.OperationRole == UserOperationRole.Operator)
                    foreach (var storeId in storeIds)
                    {
                        ResStore store = locationRepo.Find(storeId);
                        store.OperatorId = u.UserId;
                    }
            }

            ur.Commit();
            ur.Authenticate(u);
	        return u;
	    }

	    public static void SSOSignInRequestRedirect()
		{
			// Submit new login request ( current URL is sent as the "reply-to" / wreply )
			SignInRequestMessage m = new SignInRequestMessage(
				new Uri( AppContext.Current.Configuration.SSO.SigninPageUrl ),
				AppContext.Current.Configuration.SSO.RealmUri, UrlHelpers.CurrentRawUrl() );
				
			HttpContext.Current.Response.Redirect( m.WriteQueryString() );
 
			// Can also utilize POST for submitting SAML
			// HttpContext.Current.Response.Write(mess.WriteFormPost());
			// HttpContext.Current.Response.End();
		}
	}
}
