namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SlotRemoveKeys2 : DbMigration
    {
        public override void Up()
        {
            /*DropForeignKey("dbo.Tickets", "Occurrence_OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.Tickets", "ReservationType_ReservationTypeId", "dbo.ReservationTypes");
            DropIndex("dbo.Tickets", new[] { "Occurrence_OccurrenceId" });
            DropIndex("dbo.Tickets", new[] { "ReservationType_ReservationTypeId" });
            DropColumn("dbo.Tickets", "Occurrence_OccurrenceId");
            DropColumn("dbo.Tickets", "ReservationType_ReservationTypeId");*/
        }
        
        public override void Down()
        {
            /*AddColumn("dbo.Tickets", "ReservationType_ReservationTypeId", c => c.String(maxLength: 128));
            AddColumn("dbo.Tickets", "Occurrence_OccurrenceId", c => c.Int());
            CreateIndex("dbo.Tickets", "ReservationType_ReservationTypeId");
            CreateIndex("dbo.Tickets", "Occurrence_OccurrenceId");
            AddForeignKey("dbo.Tickets", "ReservationType_ReservationTypeId", "dbo.ReservationTypes", "ReservationTypeId");
            AddForeignKey("dbo.Tickets", "Occurrence_OccurrenceId", "dbo.Occurrences", "OccurrenceId");*/
        }
    }
}
