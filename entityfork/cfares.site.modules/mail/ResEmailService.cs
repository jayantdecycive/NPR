
#region Imports

using System;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Security;
using RazorEngine;
using cfacore.domain._base;
using cfares.domain._event;
using cfares.domain._event._ticket;
using cfares.domain._event._tickets;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository.contact;
using cfares.service.common;
using cfacore.shared.service.email;
using cfares.site.modules.com.application;
using cfares.site.modules.user;

#endregion

namespace cfares.site.modules.mail
{
    public class ResEmailService : EmailService
    {
		#region Properties ( Theme URL )

        protected CommunicationRepo CService;

	    private string _themeUrl;
		public override string ThemeUrl
	    {
		    get { return _themeUrl ?? ( _themeUrl = 
				HttpContext.Current.Request.RequestContext.ThemeImageUrlBase() ); }
			protected set { _themeUrl = value; }
	    }

	    #endregion

		#region Constructors

	    public ResEmailService()
	    {
	        IResContext context = ReservationConfig.GetContext();
            CService = new CommunicationRepo(context);
        }

        public ResEmailService(IResContext context)
        {
            

            CService = new CommunicationRepo(context);
        }

		#endregion

		#region Template Context Extensions

		public override string RenderText<T>( string cshtmlText, T model )
		{
			return Razor.Parse( cshtmlText, ResGetContextModel( (T) model ) );
		}

		public new ResEmailContextModel ResGetContextModel( object model ) 
		{ 
			//Type entityType = ObjectContext.GetObjectType( entry.Entity.GetType() );

			return new ResEmailContextModel {
				BaseUrl = BaseUrl,
				ThemeImagesUrl = BaseUrl + ThemeUrl,
				//Ticket = ( model is DateTicket ) ? (DateTicket) model : null,
				Ticket = ( model is ITicket ) ? (ITicket) model : null,
				User = ( model is MembershipUser ) ? (MembershipUser) model : null,
				Context = AppContext.Current
			};
		}

		#endregion

		public bool SendNewReservationNotification( ITicket ticket, bool force = false )
        {
#if DEBUG
		    return false;
#endif
            var toAddress = ticket.Slot.Occurrence.Store.Email;
            if (string.IsNullOrWhiteSpace(toAddress))
                toAddress = AppContext.Current.Configuration.Organization.AdminEmailAddress;

            if (ticket.Owner == null || string.IsNullOrWhiteSpace(toAddress))
                return false;

	        return SendMessage( new TemplatedMessage<ITicket> {
				EmailAddress = AppContext.Current.Configuration.Organization.AdminEmailAddress,
				Subject = string.Format( "New Reservation Notification - {0}", ticket.Owner.Name ),
				Model = ticket,
				ForceSend = force
		    } );
        }

        public bool SendNewReservationConfirmation( ITicket ticket, bool force = false )
        {
            if (ticket.Owner == null  || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

	        return SendMessage( new TemplatedMessage<ITicket> {
				EmailAddress = ticket.Owner.Email,
				Subject = string.Format( "Reservation Confirmation for {0}", ticket.Owner.Name ),
				Model = ticket,
				ForceSend = force,
		    } );
        }

        public bool SendPasswordReset( IResUser user, bool force = false )
        {
            var repo = new UserMembershipRepository(this.CService.Context);
            MembershipCreateStatus stat;
            string password;
            var u = repo.GetOrCreateAccount(user as ResUser,out stat, out password);
            if (u == null||stat!=MembershipCreateStatus.Success)
            {
                throw new ApplicationException( string.Format( "Unable to locate membership user [ Username = {0} ]", user.Username ) );
            }
			return SendPasswordReset( u );
        }

		public bool SendPasswordReset( MembershipUser user, bool force = false )
        {
	        return SendMessage( new TemplatedMessage<MembershipUser> {
				EmailAddress = user.Email,
				Subject = string.Format( "Reset password for {0}", user.UserName ),
				Model = user,
				ForceSend = force
		    } );
        }


        #region Email - SendMessage<T> by key: "\Views\Shared\Email\<key>.cshtml"

        public class TemplatedMessage<T>
		{
			public T Model { get; set; }
			public string Key { get; set; }
			public string Subject { get; set; }
			public string EmailAddress { get; set; }
			public bool ForceSend { get; set; }
		}

        public bool SendMessage<T>( TemplatedMessage<T> message, 
			[CallerMemberName] string callerName = "" ) where T : class
        {
	        IDomainObject domainObject = message.Model as IDomainObject;
	        if( domainObject != null )
				return SendMessage( 
					domainObject, 
					string.IsNullOrWhiteSpace( message.Key ) 
						? callerName.Replace( "Send", string.Empty ) 
						: message.Key,
					message.Subject, 
					message.EmailAddress,
					message.ForceSend,
 					domainObject.ToString() // Ticket Id not available at this time
					// .. "Ticket for Slot #138" instead typically used as unique id
				);

			return SendMessage( 
				message.Model, 
				string.IsNullOrWhiteSpace( message.Key ) 
					? callerName.Replace( "Send", string.Empty ) 
					: message.Key,
				message.Subject, 
				message.EmailAddress,
				message.ForceSend,
 				Guid.NewGuid().ToString()
			);
		}

        public bool SendMessage<T>( T model, string key, string subject, 
			string emailAddress, bool force = false, string uniqueId = "", 
			bool throwExceptionOnAlreadySent = false ) 
			where T : class
        {
			if( string.IsNullOrWhiteSpace( uniqueId ) ) uniqueId = key;

			Uri uri = new Uri( ReservationConfig.GetConfig()
				.Organization.MailUri.TrimEnd( new[] { '/' } ) + 
				key.ToLower() + "/" + uniqueId );

            if( ! force && ! CService.MailAllowed( emailAddress, uri ) )
				if( throwExceptionOnAlreadySent )
					throw new CommunicationException( "This email has already been sent." );
				else
					return true;

	        AdminEmail = AppContext.Current.Configuration.Organization.AdminEmailAddress;
			DefaultFrom = AppContext.Current.Configuration.Organization.AdminEmailAddress;

			if( string.IsNullOrWhiteSpace( DefaultFrom ) )
				throw new ArgumentException( "Invalid default from email address" );
			if( string.IsNullOrWhiteSpace( emailAddress ) )
				throw new ArgumentException( "Invalid send to email address" );

            MailMessage message = new MailMessage( new MailAddress(DefaultFrom), new MailAddress(emailAddress) )
	        {
		        Subject = subject,
		        IsBodyHtml = true,
		        Body = ViewToString<T>( key, model)
	        };

	        
            try
            {
                Client.Send( message );
                CService.MailSent( emailAddress, uri );
                return true;
            } 
			catch( Exception ex ) 
			{
                Console.Write(ex.Message);
                return false;
            }
        }

		#endregion

		#region Archive - Previous Version Email Sends

		//#region Email - ConfirmationEmail

		//public bool SendOutConfirmationEmail(TourTicket ticket) {
		//	return SendOutConfirmationEmail(ticket, false);
		//}
		//public bool SendOutConfirmationEmail(TourTicket ticket,bool force)
		//{
		//	Uri uri = new Uri("http://communication.chick-fil-a.com/res/mail/ticket/confirmation/" + ticket.Id());

		//	if (!force && !CService.MailAllowed(ticket.Owner.Email, uri))
		//		throw new CommunicationException("This email has already been sent.");

		//	const string subjectTemplate = "Reservation Confirmation for {0}";

		//	MailMessage message = new MailMessage
		//	{
		//		From = new MailAddress(DEFAULT_FROM),
		//		Subject = string.Format(subjectTemplate, ticket.Owner.Name),
		//		IsBodyHtml = true,
		//		Body = ViewToString("EmailConfirmation", ticket)
		//	};

		//	message.To.Add(new MailAddress(ticket.Owner.Email));

		//	try
		//	{
		//		Client.Send(message);
		//		CService.MailSent(ticket.Owner.Email,uri);
		//		return true;
		//	}catch(Exception ex){
		//		Console.Write(ex.Message);
		//		return false;
		//	}
		//}

		//#endregion

		//#region Email - CancellationEmails

		//public bool SendOutCancellationEmails(TourTicket ticket) { 
		//	return SendOutCancellationEmails(ticket,false);
		//}
		//public bool SendOutCancellationEmails(TourTicket ticket,bool force) {
		//	return SendOutCancellationEmailCustomer(ticket, force) && SendOutCancellationEmailAdmin(ticket);
		//}
        
		//public bool SendOutCancellationEmailCustomer(TourTicket ticket) {
		//	return SendOutCancellationEmailCustomer(ticket, false);
		//}
		//public bool SendOutCancellationEmailCustomer(TourTicket ticket, bool force)
		//{
		//	Uri uri = new Uri("http://communication.chick-fil-a.com/res/mail/ticket/cancellation/" + ticket.Id());

		//	if (!force && !CService.MailAllowed(ticket.Owner.Email, uri))
		//		throw new CommunicationException("This email has already been sent.");


		//	const string subjectTemplate = "Reservation Cancellation Notification for {0}";

		//	MailMessage message = new MailMessage
		//	{
		//		From = new MailAddress(DEFAULT_FROM),
		//		Subject = string.Format(subjectTemplate, ticket.Owner.Name),
		//		IsBodyHtml = true,
		//		Body = ViewToString("CustomerCancellationNotification", ticket)
		//	};

		//	message.To.Add(new MailAddress(ticket.Owner.Email));

		//	try
		//	{
		//		Client.Send(message);
		//		CService.MailSent(ticket.Owner.Email, uri);
		//		return true;
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.Write(ex.Message);
		//		return false;
		//	}
		//}

		//#endregion

		//#region Email - CancellationEmailAdmin

		//public bool SendOutCancellationEmailAdmin(TourTicket ticket) {
		//	return SendOutCancellationEmailAdmin(ticket,false);
		//}
		//public bool SendOutCancellationEmailAdmin(TourTicket ticket,bool force)
		//{
		//	const string subjectTemplate = "Reservation Cancellation Notification for {0}";

		//	MailMessage message = new MailMessage
		//	{
		//		From = new MailAddress(DEFAULT_FROM),
		//		Subject = string.Format(subjectTemplate, ticket.Owner.Name),
		//		IsBodyHtml = true,
		//		Body = ViewToString("AdminCancellationNotification", ticket)
		//	};

		//	message.To.Add(new MailAddress(ADMIN_EMAIL));

		//	try
		//	{
		//		Client.Send(message);                
		//		return true;
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.Write(ex.Message);
		//		return false;
		//	}
		//}


		//public bool ResetPasswordEmail(ResUser user) {
		//	return ResetPasswordEmail(user, false);
		//}
		//public bool ResetPasswordEmail(ResUser user,bool force)
		//{
		//	//throw new NotImplementedException("Token service needs to use repository");
		//	//TokenService tk = new TokenService();
		//	//ResToken token;// = tk.GeneratePasswordToken(user);
		//	//tk.Save(token);
		//	//return ResetPasswordEmail(user,token);
		//	return false;
		//}

		//#endregion

		//#region Email - ResetPassword

		//public bool ResetPasswordEmail(ResUser user, Token token) { 
		//	return ResetPasswordEmail(user, token,false);
		//}
		//public bool ResetPasswordEmail(ResUser user, Token token,bool force)
		//{
		//	Uri uri = new Uri("http://communication.chick-fil-a.com/res/mail/account/passwordreset/"+user.Id()+"/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm"));

		//	if (!force && !CService.MailAllowed(user.Email, uri))
		//		throw new CommunicationException("This email has already been sent.");

		//	const string subjectTemplate = "Password reset for {0}";

		//	MailMessage message = new MailMessage
		//	{
		//		From = new MailAddress(DEFAULT_FROM),
		//		Subject = string.Format(subjectTemplate, user.Email),
		//		IsBodyHtml = true,
		//		Body =
		//			ViewToString<TokenAndRecipient>("ResetEmail",
		//				new TokenAndRecipient {Recipient = user, Token = token})
		//	};

		//	message.To.Add(new MailAddress(user.Email));

		//	try
		//	{
		//		Client.Send(message);
		//		CService.MailSent(user.Email, uri);
		//		return true;
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.Write(ex.Message);
		//		return false;
		//	}
		//}

		//#endregion

		//#region Email - Reminder

		//public bool SendEmailReminder(TourTicket ticket) {
		//	return SendEmailReminder(ticket, false);
		//}
		//public bool SendEmailReminder(TourTicket ticket,bool force)
		//{
		//	Uri uri = new Uri("http://communication.chick-fil-a.com/res/mail/ticket/reminder/"+ticket.Id());
            
		//	DateTimeOffset correctedStart = ticket.Slot.Start;

		//	if (!force && !CService.MailAllowed(ticket.Owner.Email, uri))
		//		throw new CommunicationException("This email has already been sent.");

		//	var message = new MailMessage {IsBodyHtml = true, From = new MailAddress(DEFAULT_FROM)};

		//	const string subjectTemplate = "Reminder of Chick-fil-A tour on {0}";
		//	message.Subject = String.Format(subjectTemplate, correctedStart);

		//	message.Body = ViewToString("EmailReminder", ticket);
		//	message.To.Add(ticket.Owner.Email);
		//	message.Bcc.Add("homeofficebackstagetour@chick-fil-a.com");            
		//	try
		//	{
		//		Client.Send(message);
		//		if(!force)
		//			CService.MailSent(ticket.Owner.Email, uri);
		//		return true;
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.Write(ex.Message);
		//		return false;
		//	}
		//}

		//#endregion

		#endregion
	}
}
