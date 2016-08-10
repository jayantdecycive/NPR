
#region Imports

using System;
using System.Data.Entity;
using cfares.domain._event;
using cfares.domain._event.slot.tours;
using cfares.domain._event._tickets;
using cfares.domain.communication;
using cfares.domain.user;
using cfacore.shared.domain.store;
using cfares.domain._event._ticket.tours;
using cfacore.shared.domain.user;
using cfacore.shared.domain.common;
using cfacore.shared.domain.media;
using cfacore.domain.user;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

using cfares.domain._event.resevent.store;
using cfares.domain._event.menu;
using System.ComponentModel.DataAnnotations.Schema;
using cfares.domain.store;
using cfares.domain._event.slot;
using cfares.domain._event.occ;
using cfares.domain._event._ticket;
using cfares.domain._event.resevent;
using cfacore.domain.store;

#endregion

namespace cfares.entity.dbcontext.res_event
{
    public class CfaResContext : DbContext, IResContext
    {
	    #region Constructors
         
		// ReSharper disable NotAccessedField.Local
	    private readonly int _appId;
		// ReSharper restore NotAccessedField.Local 

		public CfaResContext(int appId) { _appId = appId; }
	    public CfaResContext(int appId, string conn) : base(conn) { _appId = appId; }
	    public CfaResContext() { Database.SetInitializer<CfaResContext>(null); }
        public CfaResContext(string conn) : base(conn) { Database.SetInitializer<CfaResContext>(null); }

		#endregion

		#region Events

		public event EventHandler SavingChanges { 
            add { 
                ((IObjectContextAdapter)this).ObjectContext.SavingChanges += value; 
            } 
            remove { 
                ((IObjectContextAdapter)this).ObjectContext.SavingChanges -= value; 
            } 
        }

        public event ObjectMaterializedEventHandler ObjectLoaded
        {
            add
            {
                ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += value;
            }
            remove
            {
                ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized -= value;
            }
        }

		#endregion

		#region DbSets

		public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemAllowance> MenuItemAllowances { get; set; }

        public DbSet<ResEvent> ResEvents { get; set; }
        public DbSet<GiveawayEvent> WeeklyGiveawayEventEvents { get; set; }

        public DbSet<ReservationType> ReservationTypes { get; set; }
        public DbSet<ReservationCategory> ReservationCategories { get; set; }

        public DbSet<ResTemplate> ResTemplate { get; set; }

        public DbSet<Occurrence> Occurrences { get; set; }
        public DbSet<GiveawayOccurrence> GiveawayOccurrences { get; set; }
        public DbSet<DateOccurrence> DateOccurrences { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Communication> Communications { get; set; }
        public DbSet<Media> Media { get; set; }

        public DbSet<Slot> Slots { get; set; }
        public DbSet<GiveawaySlot> GiveawaySlots { get; set; }
        public DbSet<TourSlot> TourSlots { get; set; }
        public DbSet<Cameo> Cameos { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<TicketTransaction> TicketTransactions { get; set; }

        public DbSet<TicketGuests> TicketGuests { get; set; }

        public DbSet<TourTicket> TourTickets { get; set; }
        public DbSet<ResToken> Tokens { get; set; }

        public DbSet<ResSiteUrl> SiteUrls { get; set; }
        
        public DbSet<ResUser> ResUsers { get; set; }

        public DbSet<ResStore> Stores { get; set; }

        public DbSet<LocationCategory> LocationCategories { get; set; }

        public DbSet<LocationSubscription> LocationSubscriptions { get; set; }
        public DbSet<Distributor> Distributors { get; set; }

        public DbSet<Address> Addresses { get; set; }

		#endregion

		#region Ignores / Customizations

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Uri>();

            modelBuilder.Entity<Media>().HasKey(x=>x.MediaId).Property(x=>x.MediaId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Media>().HasOptional(x=>x.Owner).WithMany().HasForeignKey(x=>x.OwnerId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Media>().Ignore(p => p.MediaUri);
            modelBuilder.Entity<Media>().Ignore(p => p.Crop);
            modelBuilder.Entity<Media>().Property(x => x.CreatedDate).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            modelBuilder.Entity<ResTemplate>().HasOptional(p => p.Preview).WithMany().HasForeignKey(p=>p.PreviewId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ReservationType>().Ignore(p => p.Occurrences);
            modelBuilder.Entity<ReservationType>().Ignore(p => p.Tickets);
            modelBuilder.Entity<ReservationType>().Ignore(p => p.Url);
            modelBuilder.Entity<ReservationType>().Ignore(p => p.Urls);
            modelBuilder.Entity<ReservationType>().HasMany(p => p.SiteUrls).WithOptional(x => x.ReservationType).HasForeignKey(x => x.ResEventTypeId).WillCascadeOnDelete(true);

            modelBuilder.Entity<ResEvent>().Ignore(p => p.Urls);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.Url);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.LocationCount);

            modelBuilder.Entity<ResEvent>().HasRequired(p => p.ReservationType).WithMany(x => x.EventList).HasForeignKey(x => x.ReservationTypeId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ResEvent>().HasOptional(p => p.Category).WithMany(x => x.ResEvents).HasForeignKey(x => x.CategoryId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ResEvent>().HasOptional(p => p.Template).WithMany(x => x.ResEvents).HasForeignKey(x => x.TemplateId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ResEvent>().HasOptional(p => p.Media).WithMany().HasForeignKey(x => x.MediaId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ResEvent>().Ignore(p => p.ParticipatingStoresList);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.ParticipatingStoresListLive);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.ParticipatingStoresListViewModel);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.AvailableSlots);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.AvailableSlotsByCurrentDateTime);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.ProtoOccurrence);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.MediaUrl);

            modelBuilder.Entity<ResEvent>().Ignore(p => p.SiteEndString);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.SiteStartString);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.RegistrationStartString);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.RegistrationEndString);

            modelBuilder.Entity<ResEvent>().Ignore(p => p.Slots);

            modelBuilder.Entity<ResEvent>().Ignore(p => p.AvailableSlots);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.OccurencesDistinctByDate);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.ParticipatingStoresListViewModel);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.OrganizationName);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.OrganizationDisplayName);
            modelBuilder.Entity<ResEvent>().Ignore(p => p.Urls);

            modelBuilder.Entity<ResEvent>().HasMany(p => p.SiteUrls).WithOptional(x=>x.ResEvent).HasForeignKey(x=>x.ResEventId).WillCascadeOnDelete(true);

            modelBuilder.Entity<ResSiteUrl>().Ignore(p => p.Uri);
            modelBuilder.Entity<ResSiteUrl>().HasKey(x => x.SiteUrlId).Property(x => x.SiteUrlId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		    modelBuilder.Entity<ResSiteUrl>()
		                .HasOptional(x => x.ResEvent)
		                .WithMany(x => x.SiteUrls)
		                .HasForeignKey(x => x.ResEventId);
            modelBuilder.Entity<ResSiteUrl>()
                        .HasOptional(x => x.ReservationType)
                        .WithMany(x => x.SiteUrls)
                        .HasForeignKey(x => x.ResEventTypeId);

            modelBuilder.Entity<SpeakerEvent>().ToTable("SpeakerEvents");
		    modelBuilder.Entity<SpeakerEvent>()
		                .HasOptional(x => x.OffSiteAddress)
		                .WithMany()
		                .HasForeignKey(x => x.OffSiteAddressId);
		    modelBuilder.Entity<SpeakerEvent>()
		                .HasOptional(x => x.SpeakerMedia)
		                .WithMany()
		                .HasForeignKey(x => x.SpeakerMediaId);

            
            
            

		    modelBuilder.Entity<SpeakerEvent>().HasRequired(x => x.Schedule).WithMany().HasForeignKey(x => x.ScheduleId);

            modelBuilder.Entity<GiveawayEvent>().ToTable("GiveawayEvents");
            modelBuilder.Entity<GiveawayEvent>().HasMany(p => p.ProductAllowances).WithRequired(p => p.ResEvent).HasForeignKey(x=>x.ResEventId);
            modelBuilder.Entity<GiveawayEvent>().Ignore(x=>x.AllowedProducts);

            modelBuilder.Entity<Occurrence>().ToTable("Occurrences");
            modelBuilder.Entity<GiveawayOccurrence>().HasMany(p => p.ItemsAvailable).WithMany();

            modelBuilder.Entity<OffSiteOccurrence>().HasOptional(x=>x.OffSiteAddress).WithMany().HasForeignKey(x=>x.OffSiteAddressId);
            
            modelBuilder.Entity<GiveawayOccurrence>().ToTable("GiveawayOccurrences");
            modelBuilder.Entity<GiveawayOccurrence>().Ignore(p => p.GiveawaySlotsList);
            modelBuilder.Entity<GiveawayOccurrence>().Ignore(p => p.ItemsAvailableWithoutDefaults);
            
            /*modelBuilder.Entity<GiveawayOccurrence>().HasMany(p => p.SlotsList).WithRequired(p => p.Occurrence).Map(x =>
            {
                x.MapKey("GiveawayOccurrenceID");                
                x.ToTable("GiveawayOccurrence");
            });
            modelBuilder.Entity<GiveawaySlot>().HasRequired(x => x.Occurrence).WithMany(x => x.SlotsList);*/
            modelBuilder.Entity<GiveawaySlot>().HasMany(p => p.Items).WithMany();

            modelBuilder.Entity<Occurrence>().HasKey(x => x.OccurrenceId).Property(x => x.OccurrenceId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            
            modelBuilder.Entity<Occurrence>().HasRequired(p => p.ResEvent).WithMany(x => x.Occurrences).HasForeignKey(x => x.ResEventId).WillCascadeOnDelete(true);

            
            modelBuilder.Entity<Occurrence>().HasRequired(p => p.Store).WithMany(x=>x.Occurrences).HasForeignKey(x=>x.StoreId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.ResEventName);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.StartString);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.EndString);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.SlotRangeStartString);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.SlotRangeEndString);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.Tickets);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.GMTOffset);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.AvailableSlots);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.StoreName);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.SlotRange);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.RegistrationAvailability);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.StartStringAsDate);
            modelBuilder.Entity<Occurrence>().Ignore(p => p.StartStringAsTime);

            modelBuilder.Entity<Slot>().ToTable("Slots");            
            
            modelBuilder.Entity<Slot>().HasRequired(p => p.Occurrence).WithMany(x => x.SlotsList).HasForeignKey(x => x.OccurrenceId).WillCascadeOnDelete(true);
            
            modelBuilder.Entity<Slot>().HasOptional(p => p.Schedule).WithMany(x => x.Slots).HasForeignKey(x => x.ScheduleId).WillCascadeOnDelete(false);
            
            modelBuilder.Entity<Slot>().Ignore(x => x.TicketsAvailable);
            modelBuilder.Entity<Slot>().Ignore(x => x.TotalTickets);
            modelBuilder.Entity<Slot>().Ignore(x => x.StoreId);
            modelBuilder.Entity<Slot>().Ignore(x => x.StartOffsetString);
            modelBuilder.Entity<Slot>().Ignore(x => x.EndOffsetString);
            modelBuilder.Entity<Slot>().Ignore(x => x.IssuedTickets);
            modelBuilder.Entity<Slot>().Ignore(x => x.CutoffString);
            modelBuilder.Entity<Slot>().Ignore(x => x.StartOffsetDateString);
            modelBuilder.Entity<Slot>().Ignore(x => x.CapacityScheduled);
            modelBuilder.Entity<Slot>().Ignore(x => x.EndOffsetDateString);
            

            modelBuilder.Entity<TourSlot>().ToTable("TourSlots");

            modelBuilder.Entity<TourSlot>().HasMany(x => x.Cameos).WithRequired(x=>x.TourSlot).HasForeignKey(x=>x.TourSlotId).WillCascadeOnDelete(true);
            modelBuilder.Entity<TourSlot>().Ignore(x=>x.CameoSets);
            modelBuilder.Entity<TourSlot>().HasOptional(x => x.Guide).WithMany(x => x.GuideForSlots).HasForeignKey(x=>x.GuideId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Cameo>().HasRequired(x => x.ResUser).WithMany().HasForeignKey(x=>x.ResUserId).WillCascadeOnDelete(true);
            modelBuilder.Entity<Cameo>().HasKey(x => new { x.ResUserId, x.TourSlotId });
            
            //modelBuilder.Entity<GiveawaySlot>().ToTable("FoodSlots");
            modelBuilder.Entity<GiveawaySlot>().HasMany(p => p.Items).WithMany();
            modelBuilder.Entity<TourTicket>().ToTable("TourTickets");

            modelBuilder.Entity<MenuItemAllowance>().HasKey(x => new
                {
                    x.ResEventId,x.AllowedItemId
                });

		    modelBuilder.Entity<MenuItem>().HasKey(x => x.DomId);
            modelBuilder.Entity<MenuItem>().Ignore(x=>x.AppliedEvents);
		    modelBuilder.Entity<MenuItem>()
		                .HasMany(x => x.EventAllowances)
		                .WithRequired(x => x.AllowedItem)
		                .HasForeignKey(x => x.AllowedItemId);

            modelBuilder.Entity<MenuItem>().HasOptional(p => p.Media).WithMany().HasForeignKey(x => x.MediaId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>().ToTable("Tickets");            
            modelBuilder.Entity<Ticket>().HasOptional(x=>x.Owner).WithMany(x=>x.Tickets).HasForeignKey(x=>x.OwnerId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Ticket>().HasOptional(p => p.Slot).WithMany(x=>x.Tickets).HasForeignKey(x=>x.SlotId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Ticket>().Ignore(x => x.OwnerName);
            modelBuilder.Entity<Ticket>().Ignore(x => x.OwnerEmail);
            modelBuilder.Entity<Ticket>().Ignore(x => x.OwnerHomePhone);
            modelBuilder.Entity<Ticket>().Ignore(x => x.AdditionalInformation);



            modelBuilder.Entity<DateTicket>().Ignore(x => x.SlotStart);
            modelBuilder.Entity<DateTicket>().Ignore(x => x.OwnerZip);

            modelBuilder.Entity<FoodTicket>().Ignore(x => x.MenuItemName);
            modelBuilder.Entity<FoodTicket>().HasRequired(p => p.Item).WithMany().HasForeignKey(x => x.MenuItemId).WillCascadeOnDelete(true);
            modelBuilder.Entity<TourTicket>().Ignore(x => x.Guests);

		    modelBuilder.Entity<TicketTransaction>()
		                .Property(x => x.TicketTransactionId)
		                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		    modelBuilder.Entity<TicketTransaction>()
		                .HasKey(x => x.TicketTransactionId);

            modelBuilder.Entity<TicketTransaction>()
                        .HasOptional(x => x.Customer).WithMany().HasForeignKey(x=>x.CustomerId);
            modelBuilder.Entity<TicketTransaction>().HasOptional(x => x.TicketGuest).WithMany().HasForeignKey(x => x.TicketGuestsId);

		    modelBuilder.Entity<TicketTransaction>().HasRequired(x=>x.Ticket).WithMany(x=>x.Transactions).HasForeignKey(x=>x.TicketId);

            modelBuilder.Entity<TicketGuests>().Property(x => x.TicketGuestsId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TicketGuests>().HasKey(x => x.TicketGuestsId);
            modelBuilder.Entity<TicketGuests>().HasRequired(x => x.Ticket).WithMany(x=>x.TicketGuests).HasForeignKey(x => x.TicketId);
            
            modelBuilder.Entity<Address>().Ignore(p => p.Zip);
            modelBuilder.Entity<Address>().Ignore(p => p.Name);
            modelBuilder.Entity<Address>().Ignore(p => p.Coordinates);

		    modelBuilder.Entity<ResToken>().HasKey(x => x.TokenUID);
            modelBuilder.Entity<ResToken>().HasRequired(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Communication>().Ignore(x=>x.EmailUri);

            modelBuilder.Entity<ResUser>().ToTable("ResUsers");
            modelBuilder.Entity<ResUser>().Ignore(x => x.UserPreferredLocations);

		    modelBuilder.Entity<ResUser>().HasOptional(x => x.Address).WithMany().HasForeignKey(x => x.AddressId);

            modelBuilder.Entity<ResUser>().Ignore(x => x.BirthDate);
            modelBuilder.Entity<ResUser>().Ignore(x => x.BirthMonth);

            modelBuilder.Entity<User>().Ignore(x => x.FirstName);
            modelBuilder.Entity<ResUser>().Ignore(x => x.LastActivityString);
            modelBuilder.Entity<User>().Ignore(x => x.LastName);
            modelBuilder.Entity<ResUser>().HasMany(p => p.UserPreferredLocationSubscriptions)
                .WithRequired(p => p.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(true); 
            modelBuilder.Entity<User>().Ignore(p => p.Name);
            modelBuilder.Entity<User>().Ignore(p => p.MobilePhone);
            modelBuilder.Entity<User>().Ignore(p => p.HomePhone);
            modelBuilder.Entity<User>().Ignore(p => p.ZipCode);
            modelBuilder.Entity<User>().Ignore(p => p.BirthDayString);
            modelBuilder.Entity<User>().Ignore(x => x.Creation);

            modelBuilder.Entity<User>()
                .Property(e => e.Email).HasMaxLength(250);
            modelBuilder.Entity<User>()
                .Property(e => e.Username).HasMaxLength(250);

            modelBuilder.Entity<User>()
                .Property(e => e.Authority).HasMaxLength(250);

            modelBuilder.Entity<User>()
                .Property(e => e.AuthorityUID).HasMaxLength(250);

            modelBuilder.Entity<ResStore>().ToTable("Stores");
            modelBuilder.Entity<ResStore>().Ignore(p => p.MarketableURL);
            modelBuilder.Entity<ResStore>().Ignore(p => p.OperatorName);
            modelBuilder.Entity<ResStore>().Ignore(p => p.City);
            modelBuilder.Entity<ResStore>().Ignore(p => p.EventParticipationList);
            
            modelBuilder.Entity<ResStore>().HasOptional(p => p.Category).WithMany(x=>x.Locations).HasForeignKey(x => x.CategoryId).WillCascadeOnDelete(false);            

            modelBuilder.Entity<ResStore>().HasOptional(p => p.BusinessConsultant).WithMany().HasForeignKey(x => x.BusinessConsultantId).WillCascadeOnDelete(false);            
            modelBuilder.Entity<ResStore>().HasOptional(p => p.MarketingConsultant).WithMany().HasForeignKey(x => x.MarketingConsultantId).WillCascadeOnDelete(false);            
            modelBuilder.Entity<ResStore>().HasOptional(p => p.LocationContact).WithMany().HasForeignKey(x => x.LocationContactId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ResStore>().HasOptional(p => p.Operator).WithMany().HasForeignKey(x => x.OperatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ResStore>().HasOptional(p => p.UnitMarketingDirector).WithMany().HasForeignKey(x => x.UnitMarketingDirectorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ResStore>().HasOptional(p => p.Distributor).WithMany().HasForeignKey(x => x.DistributorId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ResStore>().HasOptional(p => p.StreetAddress).WithMany().HasForeignKey(x => x.StreetAddressId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ResStore>().HasOptional(p => p.ShippingAddress).WithMany().HasForeignKey(x => x.ShippingAddressId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ResStore>().HasOptional(p => p.BillingAddress).WithMany().HasForeignKey(x => x.BillingAddressId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ResStore>().Ignore(x => x.GMTOffsetTimeZoneInfo);
            modelBuilder.Entity<ResStore>().Ignore(x => x.EventParticipationList);
            modelBuilder.Entity<ResStore>().HasMany(p => p.PreferredUserSubscriptions)
                .WithRequired(p => p.Store).HasForeignKey(x=>x.StoreId).WillCascadeOnDelete(true);
            modelBuilder.Entity<ResStore>().Ignore(x => x.PreferredUsers);


            modelBuilder.Entity<LocationCategory>().HasKey(x => x.LocationCategoryId).Property(x => x.LocationCategoryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ReservationCategory>().HasKey(x => x.ReservationCategoryId).Property(x => x.ReservationCategoryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LocationSubscription>().HasKey(x => new { x.StoreId, x.UserId });

            
            modelBuilder.Entity<ResStore>().Ignore(p => p.Fax);
            modelBuilder.Entity<ResStore>().Ignore(p => p.Phone);
            modelBuilder.Entity<ResStore>().Ignore(p => p.VoiceMail);
            
            modelBuilder.Entity<ResStore>().Ignore(p => p.Features);
            
            modelBuilder.Entity<ResStore>().HasOptional (p => p.FinancialConsultant).WithMany().HasForeignKey(x=>x.FinancialConsultantId);
            
            
            modelBuilder.Entity<ResStore>().Ignore(p => p.MapUrl);
            modelBuilder.Entity<ResStore>().Ignore(p => p.Coordinates);
            
            
            
            modelBuilder.Entity<Communication>().Ignore(p => p.EmailUri);

            modelBuilder.Entity<LocationCategory>().Ignore(p => p.StrType);
            //modelBuilder.Entity<ReservationCategory>().Ignore(p => p.);
		}

		#endregion
	}
}
