namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_birthday1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResTokens",
                c => new
                    {
                        TokenUID = c.Guid(nullable: false),
                        UserId = c.Int(),
                        Action = c.String(),
                        Expiration = c.DateTime(nullable: false),
                        Data = c.String(),
                        User_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TokenUID)
                .ForeignKey("dbo.ResUsers", t => t.User_UserId)
                .Index(t => t.User_UserId);
            
            AddColumn("dbo.Tickets", "EmailConfirmation", c => c.String());
            AddColumn("dbo.Stores", "ConceptCode", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "LocationCode", c => c.Int(nullable: false));
            DropColumn("dbo.ResUsers", "EmailJoinNewsletters");
            DropColumn("dbo.ResUsers", "EmailJoinPromotions");
            DropColumn("dbo.ResUsers", "OfAge");
            DropColumn("dbo.ResUsers", "Birthday");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResUsers", "Birthday", c => c.DateTime(nullable: false));
            AddColumn("dbo.ResUsers", "OfAge", c => c.Boolean(nullable: false));
            AddColumn("dbo.ResUsers", "EmailJoinPromotions", c => c.Boolean(nullable: false));
            AddColumn("dbo.ResUsers", "EmailJoinNewsletters", c => c.Boolean(nullable: false));
            DropIndex("dbo.ResTokens", new[] { "User_UserId" });
            DropForeignKey("dbo.ResTokens", "User_UserId", "dbo.ResUsers");
            DropColumn("dbo.Stores", "LocationCode");
            DropColumn("dbo.Stores", "ConceptCode");
            DropColumn("dbo.Tickets", "EmailConfirmation");
            DropTable("dbo.ResTokens");
        }
    }
}
