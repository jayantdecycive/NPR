namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_price_group : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stores", "PriceGroupNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stores", "PriceGroupNumber", c => c.String(nullable: false));
        }
    }
}
