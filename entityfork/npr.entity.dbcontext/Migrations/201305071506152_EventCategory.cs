namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReservationCategories",
                c => new
                    {
                        ReservationCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        StrType = c.String(),
                    })
                .PrimaryKey(t => t.ReservationCategoryId);
            
            AddColumn("dbo.ResEvents", "CategoryId", c => c.Int());
            AddForeignKey("dbo.ResEvents", "CategoryId", "dbo.ReservationCategories", "ReservationCategoryId");
            CreateIndex("dbo.ResEvents", "CategoryId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ResEvents", new[] { "CategoryId" });
            DropForeignKey("dbo.ResEvents", "CategoryId", "dbo.ReservationCategories");
            DropColumn("dbo.ResEvents", "CategoryId");
            DropTable("dbo.ReservationCategories");
        }
    }
}
