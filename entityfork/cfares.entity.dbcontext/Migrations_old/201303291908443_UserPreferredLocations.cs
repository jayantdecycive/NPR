namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPreferredLocations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResUserResStores",
                c => new
                    {
                        ResUser_UserId = c.Int(nullable: false),
                        ResStore_LocationNumber = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ResUser_UserId, t.ResStore_LocationNumber })
                .ForeignKey("dbo.ResUsers", t => t.ResUser_UserId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.ResStore_LocationNumber, cascadeDelete: true)
                .Index(t => t.ResUser_UserId)
                .Index(t => t.ResStore_LocationNumber);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ResUserResStores", new[] { "ResStore_LocationNumber" });
            DropIndex("dbo.ResUserResStores", new[] { "ResUser_UserId" });
            DropForeignKey("dbo.ResUserResStores", "ResStore_LocationNumber", "dbo.Stores");
            DropForeignKey("dbo.ResUserResStores", "ResUser_UserId", "dbo.ResUsers");
            DropTable("dbo.ResUserResStores");
        }
    }
}
