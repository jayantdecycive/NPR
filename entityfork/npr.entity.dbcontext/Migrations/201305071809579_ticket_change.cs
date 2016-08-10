namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticket_change : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "GuestListString", c => c.String());
            AddColumn("dbo.Tickets", "DatesString", c => c.String());
            DropColumn("dbo.Tickets", "GuestList");
            DropColumn("dbo.Tickets", "Email");
            DropColumn("dbo.Tickets", "EmailConfirm");
            DropColumn("dbo.Tickets", "Phone_Number");
            DropColumn("dbo.Tickets", "Phone_Full");
            DropColumn("dbo.Tickets", "Phone_AreaCode");
            DropColumn("dbo.Tickets", "Phone_Extension");
            DropColumn("dbo.Tickets", "Phone_Type");
            DropColumn("dbo.Tickets", "Phone_Carrier");
            DropColumn("dbo.Tickets", "ContactNotes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "ContactNotes", c => c.String());
            AddColumn("dbo.Tickets", "Phone_Carrier", c => c.String());
            AddColumn("dbo.Tickets", "Phone_Type", c => c.String());
            AddColumn("dbo.Tickets", "Phone_Extension", c => c.String());
            AddColumn("dbo.Tickets", "Phone_AreaCode", c => c.Int());
            AddColumn("dbo.Tickets", "Phone_Full", c => c.Long());
            AddColumn("dbo.Tickets", "Phone_Number", c => c.Int());
            AddColumn("dbo.Tickets", "EmailConfirm", c => c.String());
            AddColumn("dbo.Tickets", "Email", c => c.String());
            AddColumn("dbo.Tickets", "GuestList", c => c.String());
            DropColumn("dbo.Tickets", "DatesString");
            DropColumn("dbo.Tickets", "GuestListString");
        }
    }
}
