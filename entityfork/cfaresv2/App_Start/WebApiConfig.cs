using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.Routing;
using Microsoft.Data.Edm;
using cfacore.domain.store;
using cfacore.domain.user;
using cfacore.shared.domain.media;
using cfacore.shared.domain.store;
using cfacore.shared.domain.user;
using cfares.domain._event;
using cfares.domain._event.menu;
using cfares.domain._event.occ;
using cfares.domain._event.resevent;
using cfares.domain._event.resevent.store;
using cfares.domain._event.slot.tours;
using cfares.domain._event._ticket;
using cfares.domain._event._ticket.tours;
using cfares.domain._event._tickets;
using cfares.domain.store;
using cfares.domain.user;
using npr.domain._event.slot;
using npr.domain._event.ticket;

namespace cfaresv2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableQuerySupport();

            /*var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);*/

            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
            EntitySetConfiguration<Store> products = modelBuilder.EntitySet<Store>("Store");
            modelBuilder.EntitySet<ResStore>("ResStore");
            modelBuilder.EntitySet<Address>("Address");

            modelBuilder.EntitySet<User>("User");

            modelBuilder.EntitySet<ResUser>("ResUser");

            modelBuilder.EntitySet<Distributor>("Distributor");
            modelBuilder.EntitySet<Schedule>("Schedule");
            modelBuilder.EntitySet<Slot>("Slot");
            modelBuilder.EntitySet<Ticket>("Ticket");
            modelBuilder.EntitySet<GuestTicket>("GuestTicket");
            modelBuilder.EntitySet<TicketGuests>("TicketGuests");

            modelBuilder.EntitySet<TicketTransaction>("TicketTransaction");

            modelBuilder.EntitySet<FoodTicket>("FoodTicket");
            modelBuilder.EntitySet<DateTicket>("DateTicket");
            modelBuilder.EntitySet<NPRTicket>("NPRTicket");
            modelBuilder.EntitySet<NPRSlot>("NPRSlot");
            modelBuilder.EntitySet<TourSlot>("TourSlot");
            modelBuilder.EntitySet<TourTicket>("TourTicket");
            modelBuilder.EntitySet<Occurrence>("Occurrence");
            modelBuilder.EntitySet<DateOccurrence>("DateOccurrence");

            modelBuilder.EntitySet<ResEvent>("Event");

            modelBuilder.EntitySet<MenuItemAllowance>("MenuItemAllowance");

            modelBuilder.EntitySet<Media>("Media");
            modelBuilder.EntitySet<ResTemplate>("Template");
            modelBuilder.EntitySet<ReservationType>("ReservationType");
            modelBuilder.EntitySet<ReservationCategory>("ReservationCategory");

            modelBuilder.EntitySet<ResSiteUrl>("SiteUrl");
            modelBuilder.EntitySet<LocationSubscription>("LocationSubscription");

            
            modelBuilder.EntitySet<MenuItem>("MenuItem");


            modelBuilder.EntitySet<LocationCategory>("LocationCategory");

            modelBuilder.EntitySet<Cameo>("Cameos");

            modelBuilder.Entity<ResUser>().DerivesFrom<User>();
            modelBuilder.Entity<TourSlot>().DerivesFrom<Slot>();
            //modelBuilder.Entity<Slot>().HasRequired(x => x.Occurrence);
            modelBuilder.Entity<TourTicket>().DerivesFrom<Ticket>();
            modelBuilder.Entity<DateTicket>().DerivesFrom<Ticket>();
            modelBuilder.Entity<FoodTicket>().DerivesFrom<Ticket>();

            modelBuilder.Entity<GiveawayEvent>().DerivesFrom<ResEvent>();
            modelBuilder.Entity<GiveawayOccurrence>().DerivesFrom<Occurrence>();


            /*
             * MDRAKE: This doesn't seem optimal, but I'm placing all models for all applications here
             * */

            /*NPR*/
			//modelBuilder.EntitySet<NPRLocation>("EventLocations");
			//modelBuilder.EntitySet<NPRResEvent>("MediaEvents");
            /*END NPR*/

            IEdmModel model = modelBuilder.GetEdmModel();
            config.Routes.MapODataRoute("ResODataApi", "api", model);

			config.Routes.MapHttpRoute(
                name: "ResApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new {action="Get", controller="SlotService", id = RouteParameter.Optional}
                );

			//config.Routes.MapHttpRoute(
			//	name: "ResApi",
			//	routeTemplate: "api/{controller}/{action}/{id}",
			//	defaults: new {action="Get",controller="SlotService", id = RouteParameter.Optional},
			//	constraints: new { httpMethod = new HttpMethodConstraint(new HttpMethod[]
			//		{
			//			new HttpMethod("GET"),
			//			new HttpMethod("POST"),
			//			new HttpMethod("DELETE"),
			//			new HttpMethod("PUT")
			//		}) }
			//	);

            config.Routes.MapHttpRoute(
                name: "ResApiNoAction",
                routeTemplate: "api/{controller}/{id}",
                defaults: null,
                constraints: new { id = @"^\d+$" } // Only integers 
            );
        }

    }
}