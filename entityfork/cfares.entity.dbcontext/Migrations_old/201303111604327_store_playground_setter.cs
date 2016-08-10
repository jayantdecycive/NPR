namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_playground_setter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stores", "Playground", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stores", "Playground");
        }
    }
}
