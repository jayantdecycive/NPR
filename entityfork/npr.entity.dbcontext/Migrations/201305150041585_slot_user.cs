namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class slot_user : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NPRSlots", "Guide_UserId", "dbo.ResUsers");
            DropIndex("dbo.NPRSlots", new[] { "Guide_UserId" });
            AddForeignKey("dbo.NPRSlots", "GuideId", "dbo.ResUsers", "UserId");
            CreateIndex("dbo.NPRSlots", "GuideId");
            DropColumn("dbo.NPRSlots", "Guide_UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NPRSlots", "Guide_UserId", c => c.Int());
            DropIndex("dbo.NPRSlots", new[] { "GuideId" });
            DropForeignKey("dbo.NPRSlots", "GuideId", "dbo.ResUsers");
            CreateIndex("dbo.NPRSlots", "Guide_UserId");
            AddForeignKey("dbo.NPRSlots", "Guide_UserId", "dbo.ResUsers", "UserId");
        }
    }
}
