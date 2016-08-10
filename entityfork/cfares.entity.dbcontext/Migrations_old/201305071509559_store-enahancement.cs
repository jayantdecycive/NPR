namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storeenahancement : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResEvents", "CategoryId", "dbo.ReservationCategories");
            DropIndex("dbo.ResEvents", new[] { "CategoryId" });
            DropColumn("dbo.ResEvents", "CategoryId");
            DropTable("dbo.ReservationCategories");
        }
        
        public override void Down()
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
            CreateIndex("dbo.ResEvents", "CategoryId");
            AddForeignKey("dbo.ResEvents", "CategoryId", "dbo.ReservationCategories", "ReservationCategoryId");
        }
    }
}
