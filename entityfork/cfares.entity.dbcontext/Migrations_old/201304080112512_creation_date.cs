namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creation_date : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "CreatedDate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("dbo.Tickets", "CreatedDate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            DropColumn("dbo.Users", "Creation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Creation", c => c.DateTime(nullable: false));
            DropColumn("dbo.Tickets", "CreatedDate");
            DropColumn("dbo.Users", "CreatedDate");
        }
    }
}
