namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class slot_visibility : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Slots", "Visibility", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Slots", "Visibility");
        }
    }
}
