namespace cfares.entity.dbcontext.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class original_capacity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Slots", "OriginalCapacity", c => c.Int(nullable: false));
            Sql("UPDATE Slots SET OriginalCapacity = Capacity");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Slots", "OriginalCapacity");
        }
    }
}
