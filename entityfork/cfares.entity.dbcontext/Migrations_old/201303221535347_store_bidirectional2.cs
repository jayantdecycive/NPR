namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_bidirectional2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Stores", "IX_ResEvent_ResEventId");
            DropForeignKey("dbo.Stores", "FK_dbo.Stores_dbo.ResEvents_ResEvent_ResEventId");
            DropColumn("dbo.Stores", "ResEvent_ResEventId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stores", "ResEvent_ResEventId",c=>c.Int(nullable:true));
            AddForeignKey("dbo.Stores", "ResEvent_ResEventId", "dbo.ResEvent", "ResEventId", false, "FK_dbo.Stores_dbo.ResEvents_ResEvent_ResEventId");
            CreateIndex("dbo.Stores", "ResEvent_ResEventId", false, "IX_ResEvent_ResEventId");
        }
    }
}
