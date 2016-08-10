namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MenuItem1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MenuItems",
                c => new
                    {
                        MenuItemId = c.Int(nullable: false, identity: true),
                        DomId = c.String(),
                        Name = c.String(),
                        ImageUrl = c.String(),
                        ShortName = c.String(),
                        URLName = c.String(),
                    })
                .PrimaryKey(t => t.MenuItemId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MenuItems");
        }
    }
}
