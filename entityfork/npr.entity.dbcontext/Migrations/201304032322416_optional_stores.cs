namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class optional_stores : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Occurrences", "StoreId", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Occurrences", "StoreId", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
