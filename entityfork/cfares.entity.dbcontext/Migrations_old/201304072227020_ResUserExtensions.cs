namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResUserExtensions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResUsers", "EmailJoinNewsletters", c => c.Boolean(nullable: false));
            AddColumn("dbo.ResUsers", "EmailJoinPromotions", c => c.Boolean(nullable: false));
            AddColumn("dbo.ResUsers", "OfAge", c => c.Boolean(nullable: false));
			AddColumn("dbo.ResUsers", "Birthday", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tickets", "TableRequest", c => c.Boolean());
			DropColumn("dbo.Users", "BirthDay");
        }
        
        public override void Down()
        {
			AddColumn("dbo.Users", "BirthDay", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tickets", "TableRequest", c => c.String());
			DropColumn("dbo.ResUsers", "Birthday");
            DropColumn("dbo.ResUsers", "OfAge");
            DropColumn("dbo.ResUsers", "EmailJoinPromotions");
            DropColumn("dbo.ResUsers", "EmailJoinNewsletters");
        }
    }
}
