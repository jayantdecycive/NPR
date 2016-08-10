namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class slot_notes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Slots", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Slots", "Notes");
        }
    }
}
