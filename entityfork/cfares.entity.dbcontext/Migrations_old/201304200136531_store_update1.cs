namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_update1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Stores", "LastUpdate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stores", "LastUpdate", c => c.DateTime(nullable: false));
        }
    }
}
