namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class media_owner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "Owner_UserId", c => c.Int());
            AddForeignKey("dbo.Media", "Owner_UserId", "dbo.Users", "UserId");
            CreateIndex("dbo.Media", "Owner_UserId");
            DropColumn("dbo.Media", "OwnerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Media", "OwnerId", c => c.String());
            DropIndex("dbo.Media", new[] { "Owner_UserId" });
            DropForeignKey("dbo.Media", "Owner_UserId", "dbo.Users");
            DropColumn("dbo.Media", "Owner_UserId");
        }
    }
}
