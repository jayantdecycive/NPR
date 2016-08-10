namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class res_new : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Addresses", "ZipString", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Addresses", "ZipString", c => c.String());
        }
    }
}
