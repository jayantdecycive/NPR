using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;

namespace cfaresv2.AuthenticationSP.App_Code
{
    public class CustomSecurityTokenService : SecurityTokenService
    {
        public CustomSecurityTokenService(SecurityTokenServiceConfiguration  config)
            : base(config)
        {
        }
 
         // Returns information about the target of the token issuance
	    protected override Scope GetScope(IClaimsPrincipal principal, RequestSecurityToken request)
	    {
            Scope scope = new Scope(request.AppliesTo.Uri.OriginalString, SecurityTokenServiceConfiguration.SigningCredentials)
	        {
		        TokenEncryptionRequired = false,
		        ReplyToAddress = request.ReplyTo
	        };
		    return scope;
	    }

         // Returns an IClaimsIdentity implementation populated with claims about the user
	    protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
	    {
			ClaimsIdentity i = new ClaimsIdentity();
			i.Claims.Add(new Claim("IdP/Claim1", "Hello from the Idp"));
			return i;
		}
    }
}