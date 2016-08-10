namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stores", "LastUpdate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stores", "LastUpdate");
        }
    }
}
