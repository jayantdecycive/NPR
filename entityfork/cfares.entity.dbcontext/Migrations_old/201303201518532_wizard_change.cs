namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wizard_change : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ResEvents", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ResEvents", "Name", c => c.String(nullable: false));
        }
    }
}
