namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_create : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Creation", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Creation");
        }
    }
}
