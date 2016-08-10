namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class res_new : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "Item_MenuItemId", "dbo.MenuItems");
            DropIndex("dbo.Tickets", new[] { "Item_MenuItemId" });
            RenameColumn(table: "dbo.Tickets", name: "Item_MenuItemId", newName: "MenuItemId");
            CreateTable(
                "dbo.ResTokens",
                c => new
                    {
                        TokenUID = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false),
                        Action = c.String(),
                        Expiration = c.DateTime(nullable: false),
                        Data = c.String(),
                    })
                .PrimaryKey(t => t.TokenUID)
                .ForeignKey("dbo.ResUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Tickets", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tickets", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Addresses", "ZipString", c => c.String(nullable: false, maxLength: 100));
            AddForeignKey("dbo.Tickets", "MenuItemId", "dbo.MenuItems", "MenuItemId", cascadeDelete: true);
            CreateIndex("dbo.Tickets", "MenuItemId");
            DropColumn("dbo.Users", "Creation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Creation", c => c.DateTime(nullable: false));
            DropIndex("dbo.ResTokens", new[] { "UserId" });
            DropIndex("dbo.Tickets", new[] { "MenuItemId" });
            DropForeignKey("dbo.ResTokens", "UserId", "dbo.ResUsers");
            DropForeignKey("dbo.Tickets", "MenuItemId", "dbo.MenuItems");
            AlterColumn("dbo.Addresses", "ZipString", c => c.String());
            DropColumn("dbo.Users", "CreatedDate");
            DropColumn("dbo.Tickets", "Status");
            DropColumn("dbo.Tickets", "CreatedDate");
            DropTable("dbo.ResTokens");
            RenameColumn(table: "dbo.Tickets", name: "MenuItemId", newName: "Item_MenuItemId");
            CreateIndex("dbo.Tickets", "Item_MenuItemId");
            AddForeignKey("dbo.Tickets", "Item_MenuItemId", "dbo.MenuItems", "MenuItemId");
        }
    }
}
