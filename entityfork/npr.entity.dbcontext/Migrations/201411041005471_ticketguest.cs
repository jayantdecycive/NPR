namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticketguest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketGuests",
                c => new
                    {
                        TicketGuestsId = c.Int(nullable: false, identity: true),
                        GuestName = c.String(),
                        TicketId = c.Int(),
                    })
                .PrimaryKey(t => t.TicketGuestsId)
                .ForeignKey("dbo.Tickets", t => t.TicketId)
                .Index(t => t.TicketId);
            
            //AddColumn("dbo.Tickets", "CanceledDate", c => c.DateTime());
            //AddColumn("dbo.Tickets", "IsPaid", c => c.Boolean());
            //AddColumn("dbo.Tickets", "TicketAmount", c => c.Decimal(precision: 18, scale: 2));
            //AddColumn("dbo.Tickets", "TotalAmount", c => c.Decimal(precision: 18, scale: 2));
            //AddColumn("dbo.Tickets", "IsAuthSuccessful", c => c.Boolean());
            //AddColumn("dbo.Tickets", "AuthorizationResponse", c => c.String());
            //AddColumn("dbo.Tickets", "CCNumber", c => c.String());
            //AddColumn("dbo.Tickets", "CCExpDateMonth", c => c.Int());
            //AddColumn("dbo.Tickets", "CCExpDateYear", c => c.Int());
            //AddColumn("dbo.Tickets", "CCName", c => c.String());
            //AddColumn("dbo.Tickets", "CCType", c => c.String());
            //AddColumn("dbo.Tickets", "ListenToNprStation", c => c.String());
            //AddColumn("dbo.Tickets", "VisitorOfWebsite", c => c.Boolean());
            //AddColumn("dbo.Tickets", "Age", c => c.String());
            //AddColumn("dbo.Tickets", "Race", c => c.String());
            //AddColumn("dbo.Tickets", "TopicsOfInterest", c => c.String());
            //AddColumn("dbo.ResEvents", "IsPaid", c => c.Boolean(nullable: false));
            //AddColumn("dbo.ResEvents", "TicketAmount", c => c.Decimal(precision: 18, scale: 2));
            //DropColumn("dbo.ResEvents", "ConfigurationStatus");
            //DropColumn("dbo.ResSiteUrls", "TemplateId");
            //DropTable("dbo.Emails");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.Emails",
            //    c => new
            //        {
            //            EmailId = c.Int(nullable: false, identity: true),
            //            FromAddress = c.String(),
            //            Recipients = c.String(),
            //            CCRecipients = c.String(),
            //            BCCRecipients = c.String(),
            //            Subject = c.String(),
            //            Body = c.String(),
            //            IsHtml = c.Boolean(nullable: false),
            //            ForceSend = c.Boolean(nullable: false),
            //            UserId = c.String(),
            //            SourceUri = c.String(),
            //            ProcessStatus = c.Int(nullable: false),
            //            ProcessResponse = c.String(),
            //            ProcessDate = c.DateTimeOffset(nullable: false),
            //            CreatedDate = c.DateTimeOffset(nullable: false),
            //            UID = c.String(),
            //        })
            //    .PrimaryKey(t => t.EmailId);
            
            //AddColumn("dbo.ResSiteUrls", "TemplateId", c => c.String());
            //AddColumn("dbo.ResEvents", "ConfigurationStatus", c => c.Int(nullable: false));
            //DropIndex("dbo.TicketGuests", new[] { "TicketId" });
            //DropForeignKey("dbo.TicketGuests", "TicketId", "dbo.Tickets");
            //DropColumn("dbo.ResEvents", "TicketAmount");
            //DropColumn("dbo.ResEvents", "IsPaid");
            //DropColumn("dbo.Tickets", "TopicsOfInterest");
            //DropColumn("dbo.Tickets", "Race");
            //DropColumn("dbo.Tickets", "Age");
            //DropColumn("dbo.Tickets", "VisitorOfWebsite");
            //DropColumn("dbo.Tickets", "ListenToNprStation");
            //DropColumn("dbo.Tickets", "CCType");
            //DropColumn("dbo.Tickets", "CCName");
            //DropColumn("dbo.Tickets", "CCExpDateYear");
            //DropColumn("dbo.Tickets", "CCExpDateMonth");
            //DropColumn("dbo.Tickets", "CCNumber");
            //DropColumn("dbo.Tickets", "AuthorizationResponse");
            //DropColumn("dbo.Tickets", "IsAuthSuccessful");
            //DropColumn("dbo.Tickets", "TotalAmount");
            //DropColumn("dbo.Tickets", "TicketAmount");
            //DropColumn("dbo.Tickets", "IsPaid");
            //DropColumn("dbo.Tickets", "CanceledDate");
           
            DropIndex("dbo.TicketGuests", new[] { "TicketId" });
            DropForeignKey("dbo.TicketGuests", "TicketId", "dbo.Tickets");
            DropTable("dbo.TicketGuests");
        }
    }
}
