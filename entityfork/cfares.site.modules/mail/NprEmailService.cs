
#region Imports

using System;
//using System.Net.Mail;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Security;
using cfacore.shared.modules.helpers;
using PostageApp;
using RazorEngine;
using cfacore.domain._base;
using cfares.domain._event;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository.contact;
using cfares.service.common;
using cfacore.shared.service.email;
using cfares.site.modules.com.application;
using npr.domain._event.ticket;
using System.Collections.Generic;
using cfares.site.modules.mail;

#endregion

namespace cfares.site.modules.mail
{
    public class NprEmailService : EmailService
    {
        #region Properties ( Theme URL )

        protected CommunicationRepo CService;

        private string _themeUrl;
        public override string ThemeUrl
        {
            get
            {
                return _themeUrl ?? (_themeUrl =
                    HttpContext.Current.Request.RequestContext.ThemeImageUrlBase());
            }
            protected set { _themeUrl = value; }
        }

        #endregion

        #region Constructors

        public NprEmailService(bool bypassDataContextInitialization = false)
        {
            if (!bypassDataContextInitialization)
            {
                IResContext context = ReservationConfig.GetContext();
                CService = new CommunicationRepo(context);
            }
        }

        public NprEmailService(IResContext context)
        {
            CService = new CommunicationRepo(context);
        }

        #endregion

        #region Template Context Extensions

        public override string RenderText<T>(string cshtmlText, T model)
        {
            return Razor.Parse(cshtmlText, ResGetContextModel(model));
        }

        public NprEmailContextModel ResGetContextModel(object model)
        {
            return new NprEmailContextModel
            {
                BaseUrl = BaseUrl,
                ThemeImagesUrl = BaseUrl + ThemeUrl,
                Ticket = (model is NPRTicket) ? (NPRTicket)model : null,
                User = (model is MembershipUser) ? (MembershipUser)model : null,
                Context = AppContext.Current
            };
        }

        #endregion

        public bool SendPasswordReset(IResUser user, bool force = true)
        {
            MembershipUser u = Membership.GetUser(user.Username);
            if (u == null) throw new ApplicationException(string.Format(
               "Unable to locate membership user [ Username = {0} ]", user.Username));

            return SendPasswordReset(u);
        }

        public bool SendPasswordReset(MembershipUser user, bool force = true)
        {
            return SendMessage(new TemplatedMessage<MembershipUser>
            {
                EmailAddress = user.Email,
                Subject = string.Format("Reset password for {0}", user.UserName),
                Model = user,
                ForceSend = force
            });
        }

        public bool SendNprReservationNotification(ITicket ticket, bool force = true)
        {
            var toAddress = ticket.Slot.Occurrence.Store.Email;
            if (string.IsNullOrWhiteSpace(toAddress))
                toAddress = AppContext.Current.Configuration.Organization.AdminEmailAddress;

            if (ticket.Owner == null || string.IsNullOrWhiteSpace(toAddress))
                return false;

            return SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = AppContext.Current.Configuration.Organization.AdminEmailAddress,
                Subject = string.Format("New Reservation Notification - {0}", ticket.Owner.Name),
                Model = ticket,
                ForceSend = force,
                Template = "NprReservationNotification",
            });
        }

        public bool SendNprEventReservationConfirmation(ITicket ticket, bool force = true)
        {
            if (ticket.Owner == null || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

            return SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = ticket.Owner.Email,
                Subject = string.Format("Reservation Confirmation for {0} {1}", ticket.Owner.FirstName, ticket.Owner.LastName),
                Model = ticket,
                ForceSend = force,
                From = "NPR Events <events@npr.org>",                
                Template = "NprEventReservationConfirmation",
            });
        }

        public bool SendNprReservationConfirmation(ITicket ticket, bool force = true)
        {
            if (ticket.Owner == null || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

            return SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = ticket.Owner.Email,
                Subject = string.Format("Reservation Confirmation for {0} {1}", ticket.Owner.FirstName, ticket.Owner.LastName),
                Model = ticket,
                ForceSend = force,
                Template = "NprReservationConfirmation",
            });
        }

        public bool SendNprSpecialtyTourConfirmation(ITicket ticket, bool force = true)
        {
            if (ticket.Owner == null || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

            return SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = ticket.Owner.Email,
                Subject = string.Format("Personalized Tour Confirmation for {0} {1}", ticket.Owner.FirstName, ticket.Owner.LastName),
                Model = ticket,
                ForceSend = force,
                Template = "NprSpecialtyTourConfirmation",
            });
        }

        public bool SendNprEventReservationCancellation(ITicket ticket, bool force = true)
        {
            if (ticket.Owner == null || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

            return SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = ticket.Owner.Email,
                Subject = string.Format("NPR Reservation Cancellation for {0}", ticket.Owner.Name),
                Model = ticket,
                ForceSend = force,
                From = "NPR Events <events@npr.org>",
                Template = "NprEventReservationCancellation",
            });
        }

        public bool SendNprReservationCancellation(ITicket ticket, bool force = true)
        {
            if (ticket.Owner == null || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

            return SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = ticket.Owner.Email,
                Subject = string.Format("NPR Reservation Cancellation for {0}", ticket.Owner.Name),
                Model = ticket,
                ForceSend = force,
                Template = "NprReservationCancellation",
            });
        }

        public bool SendNprTourRequestAdmin(ITicket ticket, bool force = true)
        {
            if (ticket.Owner == null || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

            return SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = AppContext.Current.Configuration.Organization.AdminEmailAddress,
                Subject = string.Format("NPR Personalized Tour Request for {0}", ticket.Owner.Name),
                Model = ticket,
                ForceSend = force,
                Template = "NprTourRequestAdmin",
            });
        }

        public bool SendNprTourRequestCustomer(ITicket ticket, bool force = true)
        {
            if (ticket.Owner == null || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

            return SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = ticket.Owner.Email,
                Subject = string.Format("NPR Personalized Tour Request for {0} submitted", ticket.Owner.Name),
                Model = ticket,
                ForceSend = force,
                Template = "NprTourRequestCustomer",
            });
        }

        public bool SendNprPaidEventReceipt(ITicket ticket, bool force = true)
        {
            if (ticket.Owner == null || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

            return SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = ticket.Owner.Email,
                Subject = string.Format("Paid Event Receipt for {0} {1}", ticket.Owner.FirstName, ticket.Owner.LastName),
                Model = ticket,
                ForceSend = force,
                From = "NPR Events <events@npr.org>",
                Template = "NprPaidEventReceipt",
            });
        }

        public bool SendNprPaidEventRefundReceipt(ITicket ticket, bool force = true)
        {
            //System.IO.File.AppendAllText(@"c:\npr-test.log", "SendNprPaidEventRefundReceipt() - Enter\r\n");

            if (ticket.Owner == null || string.IsNullOrWhiteSpace(ticket.Owner.Email)) return false;

            //System.IO.File.AppendAllText(@"c:\npr-test.log", "SendNprPaidEventRefundReceipt() - Sending Message\r\n");

            var success = SendMessage(new TemplatedMessage<ITicket>
            {
                EmailAddress = ticket.Owner.Email,
                Subject = string.Format("Paid Event Refund for {0} {1}", ticket.Owner.FirstName, ticket.Owner.LastName),
                Model = ticket,
                ForceSend = force,
                From = "NPR Events <events@npr.org>",
                Template = "NprPaidEventRefundReceipt",
            });

            //System.IO.File.AppendAllText(@"c:\npr-test.log", String.Format("SendNprPaidEventRefundReceipt() - Message Sent (Success=>{0})\r\n", success));

            return success;
        }

        #region Email - SendMessage<T> by key: "\Views\Shared\Email\<key>.cshtml"

        public class TemplatedMessage<T>
        {
            public T Model { get; set; }
            public string Key { get; set; }
            public string Subject { get; set; }
            public string EmailAddress { get; set; }
            public bool ForceSend { get; set; }
            public string From { get; set; }
            public string Template { get; set; }
        }

        public bool SendMessage<T>(TemplatedMessage<T> message,
            [CallerMemberName] string callerName = "") where T : class
        {

            IDomainObject domainObject = message.Model as IDomainObject;
            ITicket ticketObject = message.Model as ITicket;

            if (ticketObject != null)
                return SendMessage(
                    ticketObject,
                    string.IsNullOrWhiteSpace(message.Key)
                        ? callerName.Replace("Send", string.Empty)
                        : message.Key,
                    message.Subject,
                    message.EmailAddress,
                    message.ForceSend,
                    (string.IsNullOrWhiteSpace(message.From) ? "" : message.From),
                    message.Template,
                    ticketObject.ToString() // Ticket Id not available at this time
                    // .. "Ticket for Slot #138" instead typically used as unique id
                );

            return SendMessage(
                message.Model,
                string.IsNullOrWhiteSpace(message.Key)
                    ? callerName.Replace("Send", string.Empty)
                    : message.Key,
                message.Subject,
                message.EmailAddress,
                message.ForceSend,
                (string.IsNullOrWhiteSpace(message.From) ? "" : message.From),
                message.Template,
                Guid.NewGuid().ToString()
            );
        }

        public bool SendMessage<T>(T model, string key, string subject,
            string emailAddress, bool force = true, string from = "", string template = "", string uniqueId = "",
            bool throwExceptionOnAlreadySent = false)
            where T : class
        {
            ITicket ticketObject = model as ITicket;
            if (string.IsNullOrWhiteSpace(uniqueId)) uniqueId = key;

            Uri uri = new Uri(ReservationConfig.GetConfig()
                .Organization.MailUri.TrimEnd(new[] { '/' }) +
                template.ToLower() + "/" + uniqueId);

            if (!force && !CService.MailAllowed(emailAddress, uri))
                if (throwExceptionOnAlreadySent)
                    throw new CommunicationException("This email has already been sent.");
                else
                    return true;

            AdminEmail = AppContext.Current.Configuration.Organization.AdminEmailAddress;
            DefaultFrom = AppContext.Current.Configuration.Organization.AdminEmailAddress;

            if (!String.IsNullOrEmpty(from))
                DefaultFrom = from;
            if (string.IsNullOrWhiteSpace(DefaultFrom))
                throw new ArgumentException("Invalid default from email address");
            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentException("Invalid send to email address");

            // SH - Per ticket 64432896 - Either setup SMTP or connect to PostageApp
            string htmlEmailContent = ViewToString<T>(key, model);
            string templateName = template;
            string postageAppClientKey = AppContext.Current.Configuration.Organization.PostageAppClientKey;
            string postageAppBaseUri = AppContext.Current.Configuration.Organization.PostageAppBaseUri;
            if (postageAppClientKey.IsValid() && postageAppBaseUri.IsValid())
            {
                try
                {
                    //SendPostageAppMessage(
                    //    postageAppClientKey, postageAppBaseUri, htmlEmailContent, "",
                    //    emailAddress, subject);
                    SendPostageAppMessage(
                        postageAppClientKey, postageAppBaseUri, templateName, "",
                        emailAddress, subject, model);

                    return true;
                }
                catch (Exception ex)
                {
                    //string error = String.Format("Message:{0}\r\nStack Trace:{1}\r\nInner Exception:{2}", ex.Message, ex.StackTrace, ex.InnerException == null ? "" : ex.InnerException.ToString());
                    //System.IO.File.AppendAllText(@"c:\npr-test.log", "SendMessage() - Sending Error:\r\n" + error + "\r\n\r\n");

                    Console.Write(ex.Message);
                    return false;
                }
            }

            MailMessage message = new MailMessage(new MailAddress(DefaultFrom), new MailAddress(emailAddress))
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = htmlEmailContent
            };

            // message.To.Add( new MailAddress( emailAddress ) );
            try
            {
                Client.Send(message);
                CService.MailSent(emailAddress, uri);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }

        public void SendPostageAppMessage<T>(string postageAppClientKey, string postageAppBaseUri,
            string template, string textEmailContent, string emailAddress, string subject, T model)
        {
            Client client = new Client(postageAppClientKey); // { BaseUri = postageAppBaseUri };

            //string details = String.Format("From:{0}\r\nHtml:{1}\r\n\r\nText:{2}\r\n\r\nRecipient:{3}\r\n\r\nSubject:{4}", DefaultFrom, htmlEmailContent, textEmailContent, emailAddress, subject);
            //System.IO.File.AppendAllText(@"c:\npr-test.log", "SendPostageAppMessage() - Details:\r\n" + details + "\r\n\r\n");

            try
            {
                NPRTicket ticketObject = model as NPRTicket;
                // ITicket ticketObject = model as ITicket;
                if (null != ticketObject)
                {
                    string tourtime = ticketObject.Slot == null ? "" : ticketObject.Slot.Start.ToString("hh:mmtt") + " - " + ticketObject.Slot.End.ToString("hh:mmtt");
                    string allocatedCapacity = ticketObject.GroupSize == null ? "" : Convert.ToString(ticketObject.GroupSize);
                    if (template == "NprReservationConfirmation" || template == "NprReservationNotification" || template == "NprReservationReminder" || template == "NprSpecialtyTourConfirmation")
                    {
                        client.SendMessage(new SendMessageRequest
                        {
                            From = DefaultFrom,
                            //Html = htmlEmailContent,
                            Text = textEmailContent,
                            Recipient = emailAddress,
                            Subject = subject,
                            Template = template,
                            Variables = new Dictionary<string, string>()
                        {                           
                            {"firstname",  ticketObject.Owner.FirstName },                          
                            {"lastname",  ticketObject.Owner.LastName },
                            {"tourdate",  ticketObject.Slot.Start.ToString("dddd, MMMM dd, yyyy") }, 
                            {"tourtime",  tourtime }, 
                            {"groupsize", allocatedCapacity }, 
                            {"confirmationnumber", ticketObject.ConfirmationNumber }, 
                            {"name",  ticketObject.Owner.FirstName }, 
                       }
                        });
                    }

                    string cardexpiration = ticketObject.CCExpDateMonth == null ? "" : Convert.ToString(ticketObject.CCExpDateMonth) + "/" + Convert.ToString(ticketObject.CCExpDateYear);
                    string totalAmount = ticketObject.TotalAmount == null ? "" : Convert.ToString(ticketObject.TotalAmount);
                    if (template == "NprPaidEventReceipt" || template == "NprPaidEventRefundReceipt")
                    {
                        client.SendMessage(new SendMessageRequest
                        {
                            From = DefaultFrom,
                            Text = textEmailContent,
                            Recipient = emailAddress,
                            Subject = subject,
                            Template = template,
                            Variables = new Dictionary<string, string>()
                        {                           
                            {"totalamount",totalAmount},                          
                            {"cardholder",  ticketObject.CCName },
                            {"cardexpiration", cardexpiration },                             
                            {"name",  ticketObject.Owner.FirstName }, 
                       }
                        });
                    }
                    string ticketslot = ticketObject.Slot == null ? "" : Convert.ToString(ticketObject.Slot);
                    if (template == "NprReservationCancellation")
                    {
                        client.SendMessage(new SendMessageRequest
                        {
                            From = DefaultFrom,
                            Text = textEmailContent,
                            Recipient = emailAddress,
                            Subject = subject,
                            Template = template,
                            Variables = new Dictionary<string, string>()
                        {                           
                            {"reservationtypeid",  ticketObject.Slot.Occurrence.ResEvent.ReservationTypeId },                          
                            {"ticketslot",  ticketslot },
                            {"tourdate",  ticketObject.Slot.Start.ToString("dddd, MMMM dd, yyyy") }, 
                            {"tourtime",  tourtime }, 
                            {"reseventname",  ticketObject.Slot.Occurrence.ResEventName }, 
                            {"confirmationnumber", ticketObject.ConfirmationNumber}, 
                            {"name",  ticketObject.Owner.FirstName }, 
                       }
                        });
                    }
                    string slotOccerance = ticketObject.Slot == null ? "" : Convert.ToString(ticketObject.Slot.Occurrence);
                    string slotOcceranceStore = ticketObject.Slot == null ? "" : Convert.ToString(ticketObject.Slot.Occurrence.Store);
                    string ticketid = ticketObject.TicketId == null ? "" : Convert.ToString(ticketObject.TicketId);
                    if (template == "NprEventReservationConfirmation")
                    {
                        client.SendMessage(new SendMessageRequest
                        {
                            From = DefaultFrom,
                            Text = textEmailContent,
                            Recipient = emailAddress,
                            Subject = subject,
                            Template = template,
                            Variables = new Dictionary<string, string>()
                        {                           
                            {"slotoccerance",  slotOccerance},                          
                            {"slotoccerancestore",  slotOcceranceStore},
                            {"reseventname",  ticketObject.Slot.Occurrence.ResEventName }, 
                            {"storename",  ticketObject.Slot.Occurrence.StoreName }, 
                            {"firstname",  ticketObject.Owner.FirstName }, 
                            {"lastname",  ticketObject.Owner.LastName }, 
                            {"eventdate",  ticketObject.Slot.Start.ToString("dddd, MMMM dd, yyyy") }, 
                            {"eventtime", tourtime }, 
                            {"groupsize",  allocatedCapacity }, 
                            {"confirmationnumber",  ticketObject.ConfirmationNumber}, 
                            {"name",  ticketObject.Owner.FirstName }, 
                            {"ticketid",  ticketid},
                       }
                        });
                    }
                    if (template == "NprEventReservationCancellation")
                    {
                        client.SendMessage(new SendMessageRequest
                        {
                            From = DefaultFrom,
                            Text = textEmailContent,
                            Recipient = emailAddress,
                            Subject = subject,
                            Template = template,
                            Variables = new Dictionary<string, string>()
                        {                           
                            {"reseventname",  ticketObject.Slot.Occurrence.ResEventName},                          
                            {"ticketslot",  ticketslot},
                            {"tourdate",  ticketObject.Slot.Start.ToString("dddd, MMMM dd, yyyy") }, 
                            {"confirmationnumber",  ticketObject.ConfirmationNumber },                             
                            {"name",  ticketObject.Owner.FirstName }, 
                       }
                        });
                    }
                    if (template == "NprTourRequestCustomer")
                    {
                        string[] words = ticketObject.DatesString.Split(',');
                        string date = string.Empty;
                        
                        for (int i = 1; i <= words.Length; i++)
                        {
                           
                           if (i == words.Length)
                           {
                               date = date + (Convert.ToDateTime(words[i-1].Trim())).ToString("dddd, MMMM dd, yyyy");
                           }
                           else
                           {
                               date = date + (Convert.ToDateTime(words[i-1].Trim())).ToString("dddd, MMMM dd, yyyy") + ',';
                           }
                          
                        }
                        client.SendMessage(new SendMessageRequest
                        {
                            From = DefaultFrom,
                            Text = textEmailContent,
                            Recipient = emailAddress,
                            Subject = subject,
                            Template = template,
                            Variables = new Dictionary<string, string>()
                        {                           
                            {"firstname",  ticketObject.Owner.FirstName },                          
                            {"lastname",  ticketObject.Owner.LastName },
                            {"tourdate",  date }, 
                            {"groupsize",  allocatedCapacity},
                            {"confirmationnumber",  ticketObject.ConfirmationNumber }, 
                            {"name",  ticketObject.Owner.FirstName }, 
                       }
                        });
                    }
                    if (template == "NprTourRequestAdmin")
                    {
                        client.SendMessage(new SendMessageRequest
                        {
                            From = DefaultFrom,
                            Text = textEmailContent,
                            Recipient = emailAddress,
                            Subject = subject,
                            Template = template,
                            Variables = new Dictionary<string, string>()
                        {                           
                            {"firstname",  ticketObject.Owner.FirstName },                          
                            {"lastname",  ticketObject.Owner.LastName },                            
                       }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                //string error = String.Format("Message:{0}\r\nStack Trace:{1}\r\nInner Exception:{2}", ex.Message, ex.StackTrace, ex.InnerException == null ? "" : ex.InnerException.ToString());
                //System.IO.File.AppendAllText(@"c:\npr-test.log", "SendPostageAppMessage() - Sending Error:\r\n" + error + "\r\n\r\n");

                throw;
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
