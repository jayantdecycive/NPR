
#region Imports

using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MvcDomainRouting.Code;
using cfacore.shared.modules.helpers;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using cfares.repository.ticket;
using cfares.site.modules.com.reservations.res;
using cfares.site.modules.mail;
using cfares.site.modules.com.application;

#endregion

namespace cfares.site.modules.repository.ticket
{
    public class ResTicketRepository : ResTicketRepository<Ticket> {
        public ResTicketRepository(IResContext context) : base(context) { }
    }

    public class ResTicketRepository<TTicket> : TicketRepository<TTicket>
		where TTicket:class,ITicket,new()
    {
        public ResTicketRepository(IResContext context) : base(context)
        {
	        SavedEvent += OnSavedEvent;
        }

        public string GetModifyUrl(ITicket ticket, ControllerContext request)
        {
            return GetModifyUrl(ticket, request.RequestContext);
        }

        public string GetModifyUrl(ITicket ticket, ViewContext request)
        {
            return GetModifyUrl(ticket, request.RequestContext);
        }

        public string GetModifyUrl( ITicket ticket,RequestContext request )
		{
			// For ex: http://mothersondateknight.res.local.chick-fil-a.com/Atlanta/Review/77
			// UrlHelpers.ActionUrlEventHome() >> http://mothersondateknight.res.local.chick-fil-a.com/Atlanta

            //mdrake - I can't figure this out - going to try something else
            //we're in a bad spot due to the fact that the routes don't seem to preserve the host names

		    var ResSiteUrl = ticket.Slot.Occurrence.ResEvent.ProductionUrls.FirstOrDefault();
            ResSiteUrl = ResSiteUrl ?? ticket.Slot.Occurrence.ResEvent.ReservationType.ProductionUrls.FirstOrDefault();
           // var RouteValueDictionary = new { Action= "Reservation" ,Controller= "Reservation" ,  id= ticket.TicketId  };
            
           /* if (request!=null)
            {
            
                var helper = new UrlHelper(request);
                var url = helper.RouteUrl(ResSiteUrl.GetRouteName(), RouteValueDictionary,request.HttpContext.Request.Url.Scheme);


                return url;
            }*/
            //else// if (!string.IsNullOrEmpty(ResSiteUrl.Url))
         //   {

                return ResSiteUrl.Url.TrimEnd('/') + "/Reservation/" + ticket.TicketId;
          //  }
          

		    //return ReservationWizard.ModifyStepName.ActionUrl( ticket.Slot.Occurrence.ResEvent )
		    //.AppendToPath( ticket.TicketId.ToString( CultureInfo.InvariantCulture ) ).ToString();
		}
	
	    private static void OnSavedEvent(object sender, SavedEventArgs<TTicket> savedEventArgs)
	    {
		    // Determine if we're activating ( used in logic gates below )
		    bool activatingTicket = savedEventArgs.ObjectState.State == EntityState.Added;
		    TTicket t = savedEventArgs.Instance;

			string statusFieldKey = savedEventArgs.ObjectState.GetModifiedProperties()
				.FirstOrDefault( o => o.Equals( "Status" ) ) ?? string.Empty;
			bool statusFieldModified = statusFieldKey != string.Empty;

			// If status not modified or new, pull rip cord
			if( ! activatingTicket && ! statusFieldModified ) return;

			// If going from non-live to live, send notifications / confirmations
		    bool statusFieldActivating = activatingTicket;
			if( statusFieldModified )
			{
				TicketStatus origStatus = (TicketStatus) savedEventArgs.ObjectState.OriginalValues[ statusFieldKey ];
				TicketStatus currStatus = (TicketStatus) savedEventArgs.ObjectState.CurrentValues[ statusFieldKey ];
				statusFieldActivating = origStatus != TicketStatus.Reserved && currStatus == TicketStatus.Reserved;
			}

			// -----------------------------

			// Ensure ticket reservation does not over-allocate the event
	        if (t.CardNumber == null)
	            t.CardNumber = Guid.NewGuid().ToString();

            if (t != null && t.Slot != null)
		    {
				// Don't include current ticket when checking to see if available tickets leave enough room for requested capacity
			    int ticketsAvailable = t.Slot.GetTicketsAvailableExcept(t.TicketId);
                
                //reception events only care about total event capacity not individual slots
                if((t.Slot.Occurrence.ResEvent.MaximumCapacity ?? 0) > 0){
                    ticketsAvailable = t.Slot.Occurrence.ResEvent.TicketsAvailable + t.AllocatedCapacity;
                }

                int ticketsRequested = t.AllocatedCapacity;

			    if( ticketsRequested > ticketsAvailable )
					throw new ApplicationException( string.Format( "Requested number of tickets " + 
						"exceeds those available for this event occurence [ Tickets Requested = {0}, " + 
						" Tickets Available = {1} ]", ticketsRequested, ticketsAvailable ) );
		    }

			// -----------------------------

			// Special logic for res tickets:
			// RE: http://stackoverflow.com/questions/3686688/how-to-translate-objectstateentry-originalvalues-into-entity
			// .. Check object state to determine if we are "just now" activating this ticket
			// .. Only then do we want to send email messages

			// Only send if activating
		    if( ! statusFieldActivating || t.Status == TicketStatus.Partial) return;

		    if( ReservationConfig.GetConfig().ApplicationId == "npr" )
		    {
				bool isSpecialtyTicket = savedEventArgs.ObjectState.CurrentValues[ "IsSpecialtyTicket" ].ToBoolean();
                bool isPaid = savedEventArgs.ObjectState.CurrentValues["IsPaid"].ToBoolean();
			    if( isSpecialtyTicket )
			    {
				    new NprEmailService().SendNprTourRequestAdmin(t);
				    new NprEmailService().SendNprTourRequestCustomer(t);
			    }
			    else
			    {
                    if (t.Slot != null && t.Slot.Occurrence.ResEvent.ReservationTypeId == "SpecialEvent")
                        new NprEmailService().SendNprEventReservationConfirmation(t);
                    else
					    new NprEmailService().SendNprReservationConfirmation(t);

					//no admin notification for tours
                    //new NprEmailService().SendNprReservationNotification(i);
			    }
                if (isPaid)
                {
                    new NprEmailService().SendNprPaidEventReceipt(t);
                }
		    } 
		    else 
		    {
			    new ResEmailService().SendNewReservationConfirmation(t);
			    new ResEmailService().SendNewReservationNotification(t);
		    }
	    }
    }
}
