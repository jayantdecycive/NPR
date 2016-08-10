namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_unique_keys : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.Users", "Username", c => c.String(maxLength: 250));
            AlterColumn("dbo.Users", "Authority", c => c.String(maxLength: 250));
            AlterColumn("dbo.Users", "AuthorityUID", c => c.String(maxLength: 250));
       
            
            CreateIndex("dbo.Users", "Email",true,"UIX_UserEmail");
            CreateIndex("dbo.Users", "Username", true, "UIX_Username");
            
       
        
       
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "UIX_UserEmail");
            DropIndex("dbo.Users", "UIX_Username");
            
            AlterColumn("dbo.Users", "Username", c => c.String());
            AlterColumn("dbo.Users", "Authority", c => c.String());
            AlterColumn("dbo.Users", "AuthorityUID", c => c.String());
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
            
        }
    }
}
