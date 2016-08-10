namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResEvent2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Occurrences", "ReservationType_ReservationTypeId", "dbo.ReservationTypes");
            DropIndex("dbo.Occurrences", new[] { "ReservationType_ReservationTypeId" });
            DropColumn("dbo.Occurrences", "ReservationType_ReservationTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Occurrences", "ReservationType_ReservationTypeId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Occurrences", "ReservationType_ReservationTypeId");
            AddForeignKey("dbo.Occurrences", "ReservationType_ReservationTypeId", "dbo.ReservationTypes", "ReservationTypeId");
        }
    }
}
