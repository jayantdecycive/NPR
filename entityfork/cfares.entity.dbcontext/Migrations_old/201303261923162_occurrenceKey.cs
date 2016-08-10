namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrenceKey : DbMigration
    {
        public override void Up()
        {
			// SH - Migration Fix - ALTER TABLE [dbo].[Occurrences] DROP CONSTRAINT [FK_dbo.Occurrences_dbo.ResEvents_ResEvent_ResEventId]
			// .. 'FK_dbo.Occurrences_dbo.ResEvents_ResEvent_ResEventId' is not a constraint.
			//DropForeignKey("dbo.Occurrences", "FK_dbo.Occurrences_dbo.ResEvents_ResEvent_ResEventId");

			// SH - Migration Fix - DROP INDEX [IX_ResEvent_ResEventId] ON [dbo].[Occurrences]
			// .. Cannot drop the index 'dbo.Occurrences.IX_ResEvent_ResEventId', because it does not exist or you do not have permission.
			//DropIndex("dbo.Occurrences", "IX_ResEvent_ResEventId");

			// SH - Migration Fix - EXECUTE sp_rename @objname = N'dbo.Occurrences.ResEvent_ResEventId', @newname = N'ResEventId', @objtype = N'COLUMN'
			// .. Either the parameter @objname is ambiguous or the claimed @objtype (COLUMN) is wrong.
			// .. RenameColumn("dbo.Occurrences", "ResEvent_ResEventId", "ResEventId");
            AddColumn("dbo.Occurrences", "ResEventId",c=>c.Int(nullable:true));
            AddForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents", "ResEventId");
            CreateIndex("dbo.Occurrences", "ResEventId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Occurrences", new[] { "ResEventId" });
            DropForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents");
			//RenameColumn("dbo.Occurrences", "ResEventId", "ResEvent_ResEventId");
            DropColumn("dbo.Occurrences", "ResEventId");

			//CreateIndex("dbo.Occurrences", "ResEventId", false, "IX_ResEvent_ResEventId");
			//AddForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents", "ResEventId", false, "FK_dbo.Occurrences_dbo.ResEvents_ResEvent_ResEventId");
        }
    }
}
