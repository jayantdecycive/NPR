namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timezone : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[Stores] DROP CONSTRAINT [DF__Stores__GMTOffse__3E1D39E1]");
            AlterColumn("dbo.Stores", "GMTOffsetTimeZone", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stores", "GMTOffsetTimeZone", c => c.DateTimeOffset(nullable: false));
        }
    }
}
