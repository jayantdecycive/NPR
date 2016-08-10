namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_created : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "Creation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Creation", c => c.DateTime(nullable: false));
        }
    }
}
