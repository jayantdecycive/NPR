using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using cfaresv2.AuthenticationSP.Filters;
using cfaresv2.AuthenticationSP.Models;

namespace cfaresv2.AuthenticationSP.Controllers
{
	[Authorize]
	[InitializeSimpleMembership]
	public class AccountController : Controller
	{
		//
		// GET: /Account/Login

		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			if( string.IsNullOrWhiteSpace( returnUrl ) )
				returnUrl = HttpContext.Request["wreply"] ?? string.Empty;

			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		// URLP: http://localhost:55056/WIFIdentityProvider/Account/Login

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginModel model, string returnUrl)
		{
			if( string.IsNullOrWhiteSpace( returnUrl ) )
				returnUrl = HttpContext.Request["wreply"] ?? string.Empty;

			// Random passwords are used during the internal user creation process, 
			// .. so we're going to force login for identified users
			// Previously - WebSecurity.Login( model.UserName, model.Password, persistCookie: model.RememberMe)

			if( ModelState.IsValid )
			{
				// FormsAuthentication.SetAuthCookie( model.UserName, model.RememberMe );            

				// POST data back from IDP to SP ( SAML 2.0 custom / pre-generated )

				const string saml20Envelope = @"<?xml version=""1.0"" encoding=""UTF-8""?>
					<saml2p:Response ID=""_1c6ee929fece26f376dbb0fd969366fa"" IssueInstant=""2012-08-29T19:12:43.643Z"" Version=""2.0""
									 xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol"">
						<saml2:Issuer xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"">http://authority.cfahome.com/</saml2:Issuer>
						<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">
							<SignedInfo>
								<CanonicalizationMethod Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments""/>
								<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>
								<Reference URI=""#_1c6ee929fece26f376dbb0fd969366fa"">
									<Transforms>
										<Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""/>
									</Transforms>
									<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>
									<DigestValue>jfBAVwLMfCB5qkHbzqODKQxcGj4=</DigestValue>
								</Reference>
							</SignedInfo>
							<SignatureValue>Q/OtvALX49ZPICWogS5+Y8zxSM/rKbQC2dwlXFxYlOcj3BiszsbJbkWSuFIVD/yIAcUi7e5+kJow
								R1nIcIa+qCoo0+dTfy+I/763276See1YL4+tVsS0TVT9fw/L6d9TBRRFhjkimnX4KHplQ2br3noa
								5St4/fPXMbn8ea6p6VoIjoT9shWUyC3l9Adjwj7cvs3Q5BJ2mRLaBJTW59GZlsuzSPECwun4aOuG
								oCsGDmgtjbLCdXuSQupChirzA8jA/jTQFiHJ1NOcEJ0y+QcOAL3yoeyvxUA/ZOmvyB/4lq+m4rJl
								41d15awrEUBJnPkFEnmjjJ90LKqHyA03p4AprA==
							</SignatureValue>
							<KeyInfo>
								<X509Data>
									<X509Certificate>MIIDKDCCAhCgAwIBAgIEUDJRADANBgkqhkiG9w0BAQQFADBWMQswCQYDVQQGEwJVUzELMAkGA1UE
										CBMCR0ExEDAOBgNVBAcTB0F0bGFudGExDDAKBgNVBAoTA0NGQTEMMAoGA1UECxMDVEFPMQwwCgYD
										VQQDEwNFVEEwHhcNMTIwODIwMTUwMDE2WhcNMjIwODE4MTUwMDE2WjBWMQswCQYDVQQGEwJVUzEL
										MAkGA1UECBMCR0ExEDAOBgNVBAcTB0F0bGFudGExDDAKBgNVBAoTA0NGQTEMMAoGA1UECxMDVEFP
										MQwwCgYDVQQDEwNFVEEwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCsS2G5tBoCzmnr
										HrC+4YJDVXSATIh88FzlkesnSUNsOPekjFvtS05eQJ15L1L+/V45pl/AYkZcK7g0racv0yIB0Kss
										6owH0FOdCpjwIKDaE7Q8jtQlaoJktpa2arMHCn7eKzmstoUHBX2nzLFj4enQGtzibrAF23aQ6cgy
										gXJm4k/2Wmsom1zlfjL7UKoeR68Igq//WCDxMxA9zkn4aSAFTOYrRAx6Vh5MjdWkvBa9nkt7C9D7
										vgBj1Ng6GFadzCjHjULnRNPzNxG1994KobuuQJEKEukkC4FG+5Q1ARJmCVuGbLhALoMvxf+twVcn
										ygUamW2pLRXb6vkJXUPgObW7AgMBAAEwDQYJKoZIhvcNAQEEBQADggEBACKPMul8oHztSE7zN80y
										hshhWHup8KiZjBCxsXseM9IAGKYiNSRZ7pb+FZ/lMt2i+UaXR/Sgj6T3qufJWSGxwEnQ811IutDJ
										06QkPqbRYlhfZDOJ7+AtdRcq2qyei6XHtof3EQJx/GyEH3hIjBfva/emLWg9okOptY83o5Afn9FU
										g4LrOMRTkI1kcd4JFSmzx7/18ZPnCpvm9Q3HSMHW7FYjuPSw8w9G45J4keD80oQ8tMU+xr28xQMR
										tkG1iqX6qfLjAOw3MIYThf6NFVn32L1d38vdhjBqT6iAni3Mb19cAzzlptDcXqjYpz6dqm3M4Xt7
										DxBmU/NUeharVaIEWZw=
									</X509Certificate>
								</X509Data>
							</KeyInfo>
						</Signature>
						<saml2p:Status>
							<saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success""/>
						</saml2p:Status>
						<saml2:Assertion ID=""_8eaf4c148958793ef6e3652119981a80"" IssueInstant=""2015-08-29T19:12:43.430Z"" Version=""2.0""
										 xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"">
							<saml2:Issuer>http://authority.cfahome.com/</saml2:Issuer>
						   <saml2:Subject>
								<saml2:NameID Format=""urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified"">{0}</saml2:NameID>
								<saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"">
								</saml2:SubjectConfirmation>
							</saml2:Subject>
							<saml2:Conditions NotBefore=""2012-08-29T19:11:43.430Z"" NotOnOrAfter=""2015-08-29T19:16:43.430Z"">
								<saml2:AudienceRestriction>
									<saml2:Audience/>
								</saml2:AudienceRestriction>
							</saml2:Conditions>
							<saml2:AuthnStatement AuthnInstant=""2012-08-29T19:12:43.430Z"" SessionIndex=""_8eaf4c148958793ef6e3652119981a80""
												  SessionNotOnOrAfter=""2012-08-29T19:17:43.430Z"">
								<saml2:AuthnContext>
									<saml2:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:Password</saml2:AuthnContextClassRef>
								</saml2:AuthnContext>
							</saml2:AuthnStatement>
							<saml2:AttributeStatement>
								<saml2:Attribute Name=""AuthorityUID"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified"">
									<saml2:AttributeValue xmlns:xs=""http://www.w3.org/2001/XMLSchema""
														  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""xs:string"">
										5324ac92cf2440a19d52ef66e464c86c
									</saml2:AttributeValue>
								</saml2:Attribute>
								<saml2:Attribute Name=""StoreIDs"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified"">
									<saml2:AttributeValue xmlns:xs=""http://www.w3.org/2001/XMLSchema""
														  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""xs:string""/>
								</saml2:Attribute>
								<saml2:Attribute Name=""Username"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified"">
									<saml2:AttributeValue xmlns:xs=""http://www.w3.org/2001/XMLSchema""
														  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""xs:string"">
										{0}
									</saml2:AttributeValue>
								</saml2:Attribute>
								<saml2:Attribute Name=""Authority"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified"">
									<saml2:AttributeValue xmlns:xs=""http://www.w3.org/2001/XMLSchema""
														  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""xs:string"">
										CFAPeople
									</saml2:AttributeValue>
								</saml2:Attribute>
								<saml2:Attribute Name=""EmailAddress"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified"">
									<saml2:AttributeValue xmlns:xs=""http://www.w3.org/2001/XMLSchema""
														  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""xs:string"">
										{0}@chick-fil-a.com
									</saml2:AttributeValue>
								</saml2:Attribute>
								<saml2:Attribute Name=""Role"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified"">
									<saml2:AttributeValue xmlns:xs=""http://www.w3.org/2001/XMLSchema""
														  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""xs:string"">
										STAFF_AUDIENCE
									</saml2:AttributeValue>
								</saml2:Attribute>
								<saml2:Attribute Name=""FirstName"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified"">
									<saml2:AttributeValue xmlns:xs=""http://www.w3.org/2001/XMLSchema""
														  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""xs:string"">Zheng
									</saml2:AttributeValue>
								</saml2:Attribute>
								<saml2:Attribute Name=""LastName"" NameForma=""urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified"">
									<saml2:AttributeValue xmlns:xs=""http://www.w3.org/2001/XMLSchema""
														  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""xs:string"">YUAN
									</saml2:AttributeValue>
								</saml2:Attribute>
							</saml2:AttributeStatement>
						</saml2:Assertion>
					</saml2p:Response>";

				if( ! string.IsNullOrWhiteSpace( returnUrl ) )
				{
					Response.Clear();

					StringBuilder sb = new StringBuilder();
					sb.Append("<html>");
					sb.AppendFormat(@"<body onload='document.forms[""form""].submit()'>");
					sb.AppendFormat("<form name='form' action='{0}' method='post'>",returnUrl);

					string saml = string.Format( saml20Envelope, model.UserName );

					sb.AppendFormat( "<input type='hidden' name='wa' value='{0}'>", "wsignin1.0" );
					sb.AppendFormat( "<input type='hidden' name='wresult' value='{0}'>", saml );

					sb.Append("</form>");
					sb.Append("</body>");
					sb.Append("</html>");
					Response.Write(sb.ToString());
					Response.End();

					return View();
				}
				
				return RedirectToLocal(returnUrl);
			}

			// If we got this far, something failed, redisplay form
			ModelState.AddModelError("", "The user name or password provided is incorrect.");
			return View(model);
		}

		//
		// POST: /Account/LogOff

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			WebSecurity.Logout();

			return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/Register

		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				// Attempt to register the user
				try
				{
					WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
					WebSecurity.Login(model.UserName, model.Password);
					return RedirectToAction("Index", "Home");
				}
				catch (MembershipCreateUserException e)
				{
					ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// POST: /Account/Disassociate

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Disassociate(string provider, string providerUserId)
		{
			string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
			ManageMessageId? message = null;

			// Only disassociate the account if the currently logged in user is the owner
			if (ownerAccount == User.Identity.Name)
			{
				// Use a transaction to prevent the user from deleting their last login credential
				using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
				{
					bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
					if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
					{
						OAuthWebSecurity.DeleteAccount(provider, providerUserId);
						scope.Complete();
						message = ManageMessageId.RemoveLoginSuccess;
					}
				}
			}

			return RedirectToAction("Manage", new { Message = message });
		}

		//
		// GET: /Account/Manage

		public ActionResult Manage(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
				: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
				: message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
				: "";
			ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
			ViewBag.ReturnUrl = Url.Action("Manage");
			return View();
		}

		//
		// POST: /Account/Manage

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Manage(LocalPasswordModel model)
		{
			bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
			ViewBag.HasLocalPassword = hasLocalAccount;
			ViewBag.ReturnUrl = Url.Action("Manage");
			if (hasLocalAccount)
			{
				if (ModelState.IsValid)
				{
					// ChangePassword will throw an exception rather than return false in certain failure scenarios.
					bool changePasswordSucceeded;
					try
					{
						changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
					}
					catch (Exception)
					{
						changePasswordSucceeded = false;
					}

					if (changePasswordSucceeded)
					{
						return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
					}
					else
					{
						ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
					}
				}
			}
			else
			{
				// User does not have a local password so remove any validation errors caused by a missing
				// OldPassword field
				ModelState state = ModelState["OldPassword"];
				if (state != null)
				{
					state.Errors.Clear();
				}

				if (ModelState.IsValid)
				{
					try
					{
						WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
						return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
					}
					catch (Exception e)
					{
						ModelState.AddModelError("", e);
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// POST: /Account/ExternalLogin

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/ExternalLoginCallback

		[AllowAnonymous]
		public ActionResult ExternalLoginCallback(string returnUrl)
		{
			AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
			if (!result.IsSuccessful)
			{
				return RedirectToAction("ExternalLoginFailure");
			}

			if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
			{
				return RedirectToLocal(returnUrl);
			}

			if (User.Identity.IsAuthenticated)
			{
				// If the current user is logged in add the new account
				OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
				return RedirectToLocal(returnUrl);
			}
			else
			{
				// User is new, ask for their desired membership name
				string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
				ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
				ViewBag.ReturnUrl = returnUrl;
				return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
		{
			string provider = null;
			string providerUserId = null;

			if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
			{
				return RedirectToAction("Manage");
			}

			if (ModelState.IsValid)
			{
				// Insert a new user into the database
				using (UsersContext db = new UsersContext())
				{
					UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
					// Check if user already exists
					if (user == null)
					{
						// Insert name into the profile table
						db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
						db.SaveChanges();

						OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
						OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

						return RedirectToLocal(returnUrl);
					}
					else
					{
						ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
					}
				}
			}

			ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		//
		// GET: /Account/ExternalLoginFailure

		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		[AllowAnonymous]
		[ChildActionOnly]
		public ActionResult ExternalLoginsList(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
		}

		[ChildActionOnly]
		public ActionResult RemoveExternalLogins()
		{
			ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
			List<ExternalLogin> externalLogins = new List<ExternalLogin>();
			foreach (OAuthAccount account in accounts)
			{
				AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

				externalLogins.Add(new ExternalLogin
				{
					Provider = account.Provider,
					ProviderDisplayName = clientData.DisplayName,
					ProviderUserId = account.ProviderUserId,
				});
			}

			ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
			return PartialView("_RemoveExternalLoginsPartial", externalLogins);
		}

		#region Helpers
		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
		}

		internal class ExternalLoginResult : ActionResult
		{
			public ExternalLoginResult(string provider, string returnUrl)
			{
				Provider = provider;
				ReturnUrl = returnUrl;
			}

			public string Provider { get; private set; }
			public string ReturnUrl { get; private set; }

			public override void ExecuteResult(ControllerContext context)
			{
				OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
			}
		}

		private static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			// See http://go.microsoft.com/fwlink/?LinkID=177550 for
			// a full list of status codes.
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "User name already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}
		#endregion
	}
}
