namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guest_binding : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "NumberOfGuests", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "NumberOfGuests", c => c.Int(nullable: false));
        }
    }
}
