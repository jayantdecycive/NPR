namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_birthday : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "BirthDay", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "BirthDay");
        }
    }
}
