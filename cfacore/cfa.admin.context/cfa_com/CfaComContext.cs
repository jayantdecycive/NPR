using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacom.domain.campaign;
using cfacom.domain.media;
using cfacom.domain.store;
using cfacom.domain.story;
using cfacom.domain.tag;
using cfacom.domain.user;
using cfacore.domain.user;
using cfacore.shared.domain.user;

namespace cfa.admin.context.cfa_com
{
    public class CfaComContext:DbContext
    {
        #region Constructors

		// ReSharper disable NotAccessedField.Local
	    private readonly int _appId;
		// ReSharper restore NotAccessedField.Local

		public CfaComContext(int appId) { _appId = appId; }
	    public CfaComContext(int appId, string conn) : base(conn) { _appId = appId; }
        public CfaComContext() { Database.SetInitializer<CfaComContext>(null); }
        public CfaComContext(string conn) : base(conn) { Database.SetInitializer<CfaComContext>(null); }

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

        public DbSet<Story> Stories { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ComStore> Locations { get; set; }
        public DbSet<ComUser> ComUsers { get; set; }
        public DbSet<ComMedia> Media { get; set; }
        public DbSet<Address> Addresses { get; set; }

		#endregion

		#region Ignores / Customizations

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Uri>();

            modelBuilder.Entity<ComMedia>()
                        .HasKey(x => x.MediaId)
                        .Property(x => x.MediaId)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ComMedia>().Ignore(p => p.MediaUri);


            modelBuilder.Entity<Address>().Ignore(x=>x.Zip);
            modelBuilder.Entity<Address>().Ignore(p => p.Name);
            modelBuilder.Entity<Address>().Ignore(p => p.Coordinates);


            
            modelBuilder.Entity<Campaign>().HasKey(x => x.CampaignId);//.HasMany(x => x.Stories).WithRequired(p => p.Campaign).HasForeignKey(x=>x.StoryId);
            modelBuilder.Entity<Campaign>()
                        .Property(x => x.CreatedOn)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);


            modelBuilder.Entity<Story>().HasRequired(x => x.Campaign).WithMany(x=>x.Stories).HasForeignKey(x=>x.CampaignId);
            modelBuilder.Entity<Story>().HasMany(x => x.Images).WithMany();
            modelBuilder.Entity<Story>().HasMany(x => x.Tags).WithMany(x=>x.Stories);
            modelBuilder.Entity<Story>().HasKey(x => x.StoryId);
            modelBuilder.Entity<Story>()
                        .Property(x => x.CreatedOn)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            modelBuilder.Entity<User>().Ignore(x => x.FirstName);
            modelBuilder.Entity<User>().Ignore(x => x.LastName);
            modelBuilder.Entity<User>().Ignore(p => p.Name);
            modelBuilder.Entity<User>().Ignore(p => p.MobilePhone);
            modelBuilder.Entity<User>().Ignore(p => p.HomePhone);
            modelBuilder.Entity<User>().Ignore(p => p.ZipCode);
            modelBuilder.Entity<User>().Ignore(p => p.BirthDayString);
            modelBuilder.Entity<User>().Ignore(p => p.Creation);
            modelBuilder.Entity<User>()
                .Property(e => e.Email).HasMaxLength(250);
            modelBuilder.Entity<User>()
                .Property(e => e.Username).HasMaxLength(250);
            modelBuilder.Entity<User>()
                .Property(e => e.Authority).HasMaxLength(250);
            modelBuilder.Entity<User>()
                .Property(e => e.AuthorityUID).HasMaxLength(250);
            
            modelBuilder.Entity<ComUser>().HasOptional(x => x.Image).WithMany().HasForeignKey(x => x.ImageId);



            modelBuilder.Entity<Tag>().HasKey(x => x.TagId);
            modelBuilder.Entity<Tag>()
                        .Property(x => x.CreatedOn)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            
            modelBuilder.Entity<ComStore>().Ignore(p => p.Fax);
            modelBuilder.Entity<ComStore>().Ignore(p => p.Phone);
            modelBuilder.Entity<ComStore>().Ignore(p => p.VoiceMail);
            modelBuilder.Entity<ComStore>().Ignore(p => p.Features);

            modelBuilder.Entity<ComStore>().Ignore(p => p.Features);

            modelBuilder.Entity<ComStore>().HasOptional(p => p.FinancialConsultant).WithMany().HasForeignKey(x => x.FinancialConsultantId);


            modelBuilder.Entity<ComStore>().Ignore(p => p.MapUrl);
            modelBuilder.Entity<ComStore>().Ignore(p => p.Coordinates);
            modelBuilder.Entity<ComStore>().Property(p => p.MarketableName).HasMaxLength(200);


            modelBuilder.Entity<ComStore>().ToTable("Stores");
            modelBuilder.Entity<ComStore>().Ignore(p => p.MarketableURL);
            modelBuilder.Entity<ComStore>().Ignore(p => p.MarketableUrlString);
            modelBuilder.Entity<ComStore>().HasOptional(p => p.BusinessConsultant).WithMany().HasForeignKey(x => x.BusinessConsultantId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ComStore>().HasOptional(p => p.MarketingConsultant).WithMany().HasForeignKey(x => x.MarketingConsultantId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ComStore>().HasOptional(p => p.LocationContact).WithMany().HasForeignKey(x => x.LocationContactId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ComStore>().HasOptional(p => p.Operator).WithMany().HasForeignKey(x => x.OperatorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ComStore>().HasOptional(p => p.UnitMarketingDirector).WithMany().HasForeignKey(x => x.UnitMarketingDirectorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ComStore>().HasOptional(p => p.Distributor).WithMany().HasForeignKey(x => x.DistributorId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ComStore>().HasOptional(p => p.StreetAddress).WithMany().HasForeignKey(x => x.StreetAddressId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ComStore>().HasOptional(p => p.ShippingAddress).WithMany().HasForeignKey(x => x.ShippingAddressId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ComStore>().HasOptional(p => p.BillingAddress).WithMany().HasForeignKey(x => x.BillingAddressId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ComStore>().Ignore(x => x.GMTOffsetTimeZoneInfo);

        }

        #endregion
    }
}
