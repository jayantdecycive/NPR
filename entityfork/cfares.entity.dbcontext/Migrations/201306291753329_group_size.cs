namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class group_size : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "AllocatedCapacity", c => c.Int(nullable: true));
            Sql("UPDATE dbo.Tickets SET AllocatedCapacity=1 WHERE AllocatedCapacity is NULL OR AllocatedCapacity =0");
            AlterColumn("dbo.Tickets", "AllocatedCapacity", c => c.Int(nullable: false, defaultValue: 1));
            
            AlterColumn("dbo.Tickets", "NumberOfGuests", c => c.Int(nullable: true));
            Sql("UPDATE dbo.Tickets SET NumberOfGuests=0 WHERE NumberOfGuests is NULL");
            AlterColumn("dbo.Tickets", "NumberOfGuests", c => c.Int(nullable: false, defaultValue: 0));
            
            
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "NumberOfGuests", c => c.Int());
            DropColumn("dbo.Tickets", "AllocatedCapacity");
        }
    }
}
