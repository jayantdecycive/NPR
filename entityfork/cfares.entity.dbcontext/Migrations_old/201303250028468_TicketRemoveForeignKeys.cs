namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketRemoveForeignKeys : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Tickets", "IX_Occurrence_OccurrenceId");
            DropForeignKey("dbo.Tickets", "FK_dbo.Tickets_dbo.Occurrences_Occurrence_OccurrenceId");
            DropColumn("dbo.Tickets","Occurrence_OccurrenceId");

            DropIndex("dbo.Tickets", "IX_ReservationType_ReservationTypeId");
            DropForeignKey("dbo.Tickets", "FK_dbo.Tickets_dbo.ReservationTypes_ReservationType_ReservationTypeId");
            DropColumn("dbo.Tickets", "ReservationType_ReservationTypeId");
                        
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "ReservationType_ReservationTypeId", c => c.String(maxLength: 128));
            AddColumn("dbo.Tickets", "Occurrence_OccurrenceId", c => c.Int());
            CreateIndex("dbo.Tickets", "ReservationType_ReservationTypeId");
            CreateIndex("dbo.Tickets", "Occurrence_OccurrenceId");
            AddForeignKey("dbo.Tickets", "ReservationType_ReservationTypeId", "dbo.ReservationTypes", "ReservationTypeId");
            AddForeignKey("dbo.Tickets", "Occurrence_OccurrenceId", "dbo.Occurrences", "OccurrenceId");
        }
    }
}
