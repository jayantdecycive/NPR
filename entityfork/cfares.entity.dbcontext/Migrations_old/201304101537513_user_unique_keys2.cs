namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_unique_keys2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Authority", c => c.String(maxLength: 250));
            AlterColumn("dbo.Users", "AuthorityUID", c => c.String(maxLength: 250));
            CreateIndex("dbo.Users", new string[] { "Authority", "AuthorityUID" }, true, "UIX_Authority");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "UIX_Authority");
            AlterColumn("dbo.Users", "AuthorityUID", c => c.String());
            AlterColumn("dbo.Users", "Authority", c => c.String());
            
        }
    }
}
