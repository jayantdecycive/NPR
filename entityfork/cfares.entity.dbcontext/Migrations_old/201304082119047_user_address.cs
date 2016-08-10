namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_address : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Users", name: "Address_AddressId", newName: "AddressId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Users", name: "AddressId", newName: "Address_AddressId");
        }
    }
}
