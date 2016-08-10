
#region Imports

using System;
using System.Web;
using Microsoft.IdentityModel.Tokens.Saml2;

#endregion

namespace cfares.site.modules.com.Security
{
	// Required to resolve confirmation data validation requirements:
	// .. ID4157: A Saml2SecurityToken cannot be created from the Saml2Assertion because it contains 
	// .. a SubjectConfirmationData which specifies a Recipient value. Enforcement of this value is 
	// .. not supported by default. To customize SubjectConfirmationData processing, extend 
	// .. Saml2SecurityTokenHandler and override ValidateConfirmationData."

	public class ReservationSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
	{
		protected override void ValidateConfirmationData(Saml2SubjectConfirmationData confirmationData)
		{
			Saml2SubjectConfirmationData d = confirmationData;
			DateTime now = DateTime.Now;

			if( d.NotBefore.HasValue && now < d.NotBefore.Value )
				throw new ApplicationException( string.Format( "ID4148: The ReservationSaml2SecurityToken" + 
					" is rejected because the SAML2:Assertion's NotBefore condition is not satisfied." + 
					"{0}NotBefore: '{1}' {0}Current time: '{2}'",
					Environment.NewLine, d.NotBefore.Value, now ) );

			if( d.NotOnOrAfter.HasValue && now >= d.NotOnOrAfter.Value )
				throw new ApplicationException( string.Format( "ID4148: The ReservationSaml2SecurityToken" + 
					" is rejected because the SAML2:Assertion's NotOnOrAfter condition is not satisfied." + 
					"{0}NotOnOrAfter: '{1}' {0}Current time: '{2}'",
					Environment.NewLine, d.NotOnOrAfter.Value, now ) );

			/*
             * mdrake: ignoring recipient condition
             * Uri uri = confirmationData.Recipient;
			if( HttpContext.Current != null && HttpContext.Current.Request.Url.UriMatchByHostName( uri ) )
				throw new ApplicationException( string.Format( "ID3242: The ReservationSaml2SecurityToken" + 
					" is rejected because the SAML2:Assertion's Recipient condition is not satisfied." + 
					"{0}Recipient: '{1}' {0}Current URL: '{2}'",
					Environment.NewLine, uri, HttpContext.Current.Request.Url ) );
			*/
			// Do not call - Use above validation vs. base.ValidateConfirmationData( confirmationData );
		}
	}
}
