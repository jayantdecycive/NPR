namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResUserExtensionsMerge : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "BirthDay", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tickets", "TableRequest", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "TableRequest", c => c.Boolean());
            DropColumn("dbo.Users", "BirthDay");
        }
    }
}
