namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class slotGrouping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Slots", "Grouping", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Slots", "Grouping");
        }
    }
}
