namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResEventUrlUpdate : DbMigration
    {
        public override void Up()
        {
			// SH - Migration Fix - ALTER TABLE [dbo].[ResEvents] ADD [Url] [nvarchar](max)
			// .. System.Data.SqlClient.SqlException (0x80131904): Column names in each table must be unique. Column name 'Url' in table 'dbo.ResEvents' is specified more than once.
			// AddColumn("dbo.ResEvents", "Url", c => c.String());
            // DropColumn("dbo.Media", "ExternalUriStr");
        }
        
        public override void Down()
        {
            // AddColumn("dbo.Media", "ExternalUriStr", c => c.String());
			//DropColumn("dbo.ResEvents", "Url");
        }
    }
}
