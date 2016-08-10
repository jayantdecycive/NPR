namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RelationshipOverhaul : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResEvents", "ReservationType_ReservationTypeId", "dbo.ReservationTypes");
            DropForeignKey("dbo.Slots", "Occurrence_OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.Slots", "Schedule_ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.Tickets", "Owner_UserId", "dbo.ResUsers");
            //DropForeignKey("dbo.Tickets", "SlotId", "dbo.Slots");
            DropForeignKey("dbo.Stores", "FinancialConsultant_UserId", "dbo.Users");
            DropIndex("dbo.ResEvents", new[] { "ReservationType_ReservationTypeId" });
            DropIndex("dbo.Slots", new[] { "Occurrence_OccurrenceId" });
            DropIndex("dbo.Slots", new[] { "Schedule_ScheduleId" });
            DropIndex("dbo.Tickets", new[] { "Owner_UserId" });
            DropIndex("dbo.Tickets", new[] { "SlotId" });
            DropIndex("dbo.Stores", new[] { "FinancialConsultant_UserId" });
            RenameColumn(table: "dbo.ResEvents", name: "ReservationType_ReservationTypeId", newName: "ReservationTypeId");
            RenameColumn(table: "dbo.ResEvents", name: "Template_ResTemplateId", newName: "TemplateName");
            RenameColumn(table: "dbo.Slots", name: "Occurrence_OccurrenceId", newName: "OccurrenceId");
            //RenameColumn(table: "dbo.Tickets", name: "SlotId", newName: "Slot_SlotId");
            RenameColumn(table: "dbo.Stores", name: "BusinessConsultant_UserId", newName: "BusinessConsultantId");
            RenameColumn(table: "dbo.Stores", name: "BillingAddress_AddressId", newName: "BillingAddressId");
            RenameColumn(table: "dbo.Stores", name: "ShippingAddress_AddressId", newName: "ShippingAddressId");
            RenameColumn(table: "dbo.Stores", name: "StreetAddress_AddressId", newName: "StreetAddressId");
            RenameColumn(table: "dbo.Stores", name: "FinancialConsultant_UserId", newName: "FinancialConsultantId");
            AddColumn("dbo.Slots", "ScheduleId", c => c.Int());
            AddColumn("dbo.Tickets", "OwnerId", c => c.Int());
            AddForeignKey("dbo.ResEvents", "ReservationTypeId", "dbo.ReservationTypes", "ReservationTypeId");
            AddForeignKey("dbo.Slots", "OccurrenceId", "dbo.Occurrences", "OccurrenceId", cascadeDelete: true);
            AddForeignKey("dbo.Slots", "ScheduleId", "dbo.Schedules", "ScheduleId");
            //AddForeignKey("dbo.Tickets", "SlotId", "dbo.Slots", "SlotId", cascadeDelete: true);
            AddForeignKey("dbo.Tickets", "OwnerId", "dbo.ResUsers", "UserId");
            //AddForeignKey("dbo.Tickets", "SlotId", "dbo.Slots", "SlotId");
            AddForeignKey("dbo.Stores", "FinancialConsultantId", "dbo.Users", "UserId", cascadeDelete: true);
            CreateIndex("dbo.ResEvents", "ReservationTypeId");
            CreateIndex("dbo.Slots", "OccurrenceId");
            CreateIndex("dbo.Slots", "ScheduleId");
            CreateIndex("dbo.Tickets", "SlotId");
            CreateIndex("dbo.Tickets", "OwnerId");
            //CreateIndex("dbo.Tickets", "Slot_SlotId");
            CreateIndex("dbo.Stores", "FinancialConsultantId");
            DropColumn("dbo.Slots", "Schedule_ScheduleId");
            DropColumn("dbo.Tickets", "Owner_UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "Owner_UserId", c => c.Int());
            AddColumn("dbo.Slots", "Schedule_ScheduleId", c => c.Int());
            DropIndex("dbo.Stores", new[] { "FinancialConsultantId" });
            //DropIndex("dbo.Tickets", new[] { "Slot_SlotId" });
            DropIndex("dbo.Tickets", new[] { "OwnerId" });
            DropIndex("dbo.Tickets", new[] { "SlotId" });
            DropIndex("dbo.Slots", new[] { "ScheduleId" });
            DropIndex("dbo.Slots", new[] { "OccurrenceId" });
            DropIndex("dbo.ResEvents", new[] { "ReservationTypeId" });
            DropForeignKey("dbo.Stores", "FinancialConsultantId", "dbo.Users");
            //DropForeignKey("dbo.Tickets", "Slot_SlotId", "dbo.Slots");
            DropForeignKey("dbo.Tickets", "OwnerId", "dbo.ResUsers");
            //DropForeignKey("dbo.Tickets", "SlotId", "dbo.Slots");
            DropForeignKey("dbo.Slots", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.Slots", "OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.ResEvents", "ReservationTypeId", "dbo.ReservationTypes");
            DropColumn("dbo.Tickets", "OwnerId");
            DropColumn("dbo.Slots", "ScheduleId");
            RenameColumn(table: "dbo.Stores", name: "FinancialConsultantId", newName: "FinancialConsultant_UserId");
            RenameColumn(table: "dbo.Stores", name: "StreetAddressId", newName: "StreetAddress_AddressId");
            RenameColumn(table: "dbo.Stores", name: "ShippingAddressId", newName: "ShippingAddress_AddressId");
            RenameColumn(table: "dbo.Stores", name: "BillingAddressId", newName: "BillingAddress_AddressId");
            RenameColumn(table: "dbo.Stores", name: "BusinessConsultantId", newName: "BusinessConsultant_UserId");
            //RenameColumn(table: "dbo.Tickets", name: "Slot_SlotId", newName: "SlotId");
            RenameColumn(table: "dbo.Slots", name: "OccurrenceId", newName: "Occurrence_OccurrenceId");
            RenameColumn(table: "dbo.ResEvents", name: "TemplateName", newName: "Template_ResTemplateId");
            RenameColumn(table: "dbo.ResEvents", name: "ReservationTypeId", newName: "ReservationType_ReservationTypeId");
            CreateIndex("dbo.Stores", "FinancialConsultant_UserId");
            //CreateIndex("dbo.Tickets", "SlotId");
            CreateIndex("dbo.Tickets", "Owner_UserId");
            CreateIndex("dbo.Slots", "Schedule_ScheduleId");
            CreateIndex("dbo.Slots", "Occurrence_OccurrenceId");
            CreateIndex("dbo.ResEvents", "ReservationType_ReservationTypeId");
            AddForeignKey("dbo.Stores", "FinancialConsultant_UserId", "dbo.Users", "UserId");
            //AddForeignKey("dbo.Tickets", "SlotId", "dbo.Slots", "SlotId", cascadeDelete: true);
            AddForeignKey("dbo.Tickets", "Owner_UserId", "dbo.ResUsers", "UserId");
            AddForeignKey("dbo.Slots", "Schedule_ScheduleId", "dbo.Schedules", "ScheduleId");
            AddForeignKey("dbo.Slots", "Occurrence_OccurrenceId", "dbo.Occurrences", "OccurrenceId");
            AddForeignKey("dbo.ResEvents", "ReservationType_ReservationTypeId", "dbo.ReservationTypes", "ReservationTypeId", cascadeDelete: true);
        }
    }
}
