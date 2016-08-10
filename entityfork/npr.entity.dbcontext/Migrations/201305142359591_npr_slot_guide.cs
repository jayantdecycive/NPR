namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class npr_slot_guide : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NPRSlots", "Guide_UserId", c => c.Int());
            AddForeignKey("dbo.NPRSlots", "Guide_UserId", "dbo.ResUsers", "UserId");
            CreateIndex("dbo.NPRSlots", "Guide_UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.NPRSlots", new[] { "Guide_UserId" });
            DropForeignKey("dbo.NPRSlots", "Guide_UserId", "dbo.ResUsers");
            DropColumn("dbo.NPRSlots", "Guide_UserId");
        }
    }
}
