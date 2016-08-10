namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class speakerevent2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpeakerEvents", "EndTimeTemplate", c => c.DateTimeOffset(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SpeakerEvents", "EndTimeTemplate");
        }
    }
}
