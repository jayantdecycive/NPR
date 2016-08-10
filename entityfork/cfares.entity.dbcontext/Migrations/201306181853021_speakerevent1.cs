namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class speakerevent1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpeakerEvents", "StartTimeTemplate", c => c.DateTimeOffset(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SpeakerEvents", "StartTimeTemplate");
        }
    }
}
