namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reconcile : DbMigration
    {
        public override void Up()
        {
            DropColumn(table: "dbo.ResTokens", name: "UserId");
            RenameColumn(table: "dbo.ResTokens", name: "User_UserId", newName: "UserId");
            AlterColumn("dbo.ResTokens", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.Tickets", "EmailConfirmation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "EmailConfirmation", c => c.String());
            AlterColumn("dbo.ResTokens", "UserId", c => c.Int());
            AddColumn("dbo.ResTokens", "User_UserId", c => c.Int(nullable: true));
            RenameColumn(table: "dbo.ResTokens", name: "UserId", newName: "User_UserId");
            AddColumn("dbo.ResTokens", "UserId", c => c.Int(nullable: true));
        }
    }
}
