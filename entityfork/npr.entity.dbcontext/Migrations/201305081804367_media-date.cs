namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mediadate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "CreatedDate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            DropColumn("dbo.Media", "CreationDate");
        }

        public override void Down()
        {
            AddColumn("dbo.Media", "CreationDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Media", "CreatedDate");
        }
    }
}
