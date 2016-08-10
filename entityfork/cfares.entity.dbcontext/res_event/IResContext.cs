using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using cfacore.domain.store;
using cfares.domain._event.occ;
using cfares.domain.user;

namespace cfares.entity.dbcontext.res_event
{
    public interface IResContext
    {
        event EventHandler SavingChanges;
        event ObjectMaterializedEventHandler ObjectLoaded;
        DbSet<ResUser> ResUsers { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet Set(Type entityType);
        int SaveChanges();
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbEntityEntry Entry(object entity);

        
        DbContextConfiguration Configuration { get; }

        System.Data.Entity.DbSet<cfacore.shared.domain.user.Address> Addresses { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.slot.tours.Cameo> Cameos { get; set; }
        System.Data.Entity.DbSet<cfacore.shared.domain.common.Communication> Communications { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.occ.DateOccurrence> DateOccurrences { get; set; }
        System.Data.Entity.DbSet<cfacore.shared.domain.store.Distributor> Distributors { get; set; }
        System.Data.Entity.DbSet<GiveawayOccurrence> GiveawayOccurrences { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.slot.GiveawaySlot> GiveawaySlots { get; set; }
        System.Data.Entity.DbSet<cfares.domain.user.LocationSubscription> LocationSubscriptions { get; set; }
        System.Data.Entity.DbSet<cfacore.shared.domain.media.Media> Media { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.menu.MenuItem> MenuItems { get; set; }        
        System.Data.Entity.DbSet<cfares.domain._event.Occurrence> Occurrences { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.ReservationType> ReservationTypes { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.ReservationCategory> ReservationCategories { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.ResEvent> ResEvents { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.ResTemplate> ResTemplate { get; set; }
        
        System.Data.Entity.DbSet<cfares.domain._event.Schedule> Schedules { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.resevent.ResSiteUrl> SiteUrls { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.Slot> Slots { get; set; }
        System.Data.Entity.DbSet<cfares.domain.store.ResStore> Stores { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.Ticket> Tickets { get; set; }
        System.Data.Entity.DbSet<cfares.domain.communication.ResToken> Tokens { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.slot.tours.TourSlot> TourSlots { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event._ticket.tours.TourTicket> TourTickets { get; set; }
        System.Data.Entity.DbSet<cfares.domain._event.resevent.store.GiveawayEvent> WeeklyGiveawayEventEvents { get; set; }

        System.Data.Entity.DbSet<LocationCategory> LocationCategories { get; set; }

    }
}