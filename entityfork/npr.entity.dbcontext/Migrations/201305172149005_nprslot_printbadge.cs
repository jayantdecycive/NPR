namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nprslot_paidtickets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NPRSlots", "PrintBadge", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NPRSlots", "PrintBadge");
        }
    }
}
