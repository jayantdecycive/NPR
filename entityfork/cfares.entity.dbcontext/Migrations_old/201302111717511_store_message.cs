namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_message : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stores", "Message", c => c.String());
            AlterColumn("dbo.Stores", "LocationDescription", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stores", "LocationDescription", c => c.String(nullable: false));
            AlterColumn("dbo.Stores", "Message", c => c.String(nullable: false));
        }
    }
}
