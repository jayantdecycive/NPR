namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class playground_mistake : DbMigration
    {
        public override void Up()
        {
			// SH - Migration Fix - ALTER TABLE [dbo].[Stores] ADD [Playground] [nvarchar](max) NOT NULL DEFAULT ''
			// .. System.Data.SqlClient.SqlException (0x80131904): Column names in each table must be unique. Column name 'Playground' in table 'dbo.Stores' is specified more than once.
			// AddColumn("dbo.Stores", "Playground", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            //DropColumn("dbo.Stores", "Playground");
        }
    }
}
