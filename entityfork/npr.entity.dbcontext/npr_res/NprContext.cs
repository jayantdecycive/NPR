using cfacore.domain.user;
using cfares.domain._event;
using cfares.domain._event.occ;
using cfares.domain._event.resevent.store;
using cfares.entity.dbcontext.res_event;
using npr.domain._event.slot;
using npr.domain._event.ticket;
using System.Data.Entity;

namespace npr.entity.dbcontext.npr_res
{
    public class NprContext : CfaResContext, INprContext
    {
        public NprContext() : base(0)
        {
            Database.SetInitializer<NprContext>(null);
			//Database.SetInitializer(new SeedInitializer());
        }
        public NprContext(string conn) : base(0, conn)
        {
            Database.SetInitializer<NprContext>(null);
			//Database.SetInitializer(new SeedInitializer());
        }

        public DbSet<NPRTicket> NPRTickets { get; set; }
        public DbSet<NPRSlot> NPRSlots { get; set; }

		//public class SeedInitializer : DropCreateDatabaseIfModelChanges<NprContext> 
		//{   
		//  protected override void Seed(NprContext context) 
		//  {   
		//	context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Tickets', RESEED, 154001)");
		//  }  
		//}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Occurrence>().HasOptional(p => p.Store).WithMany(x => x.Occurrences).HasForeignKey(x => x.StoreId).WillCascadeOnDelete(false);
			//modelBuilder.Entity<NPRLocation>().HasOptional(p => p.Address).WithMany().HasForeignKey(x=>x.AddressId).WillCascadeOnDelete(false);
			//modelBuilder.Entity<NPRLocation>().HasKey(x => x.NPRLocationId);

            modelBuilder.Entity<NPRTicket>().Ignore(x => x.Dates);
            modelBuilder.Entity<NPRTicket>().Ignore(x => x.Email);
            modelBuilder.Entity<NPRTicket>().Ignore(x => x.EmailConfirm);
            modelBuilder.Entity<NPRTicket>().Ignore(x => x.Phone);
            modelBuilder.Entity<NPRTicket>().Ignore(x => x.FirstName);
            modelBuilder.Entity<NPRTicket>().Ignore(x => x.LastName);
            modelBuilder.Entity<NPRTicket>().Ignore(x => x.SlotStart);
            modelBuilder.Entity<NPRTicket>().Ignore(x => x.ConfirmationNumber);

			modelBuilder.Entity<NPRTicket>().Ignore(x => x.TopicsOfInterest1);
			modelBuilder.Entity<NPRTicket>().Ignore(x => x.TopicsOfInterest2);
			modelBuilder.Entity<NPRTicket>().Ignore(x => x.TopicsOfInterest3);
			

            

            modelBuilder.Entity<NPRSlot>().HasOptional(x => x.Guide).WithMany().HasForeignKey(x => x.GuideId);
            modelBuilder.Entity<NPRSlot>().Ignore(x => x.ReservationTypeId);
            modelBuilder.Entity<NPRSlot>().Ignore(x => x.EventName);
            modelBuilder.Entity<NPRSlot>().Ignore(x => x.EventCategory);
            modelBuilder.Entity<NPRSlot>().Ignore(x => x.EventStatus);
            modelBuilder.Entity<NPRSlot>().Ignore(x => x.EventLocationName);
			modelBuilder.Entity<NPRSlot>().Ignore(x => x.StartDay);
			modelBuilder.Entity<NPRSlot>().Ignore(x => x.StartAsString);
			modelBuilder.Entity<NPRSlot>().Ignore(x => x.EndAsString);
			modelBuilder.Entity<NPRSlot>().Ignore(x => x.SlotIdAsString);
           
			modelBuilder.Entity<User>().Ignore(x => x.Creation);
        }
    }
}
