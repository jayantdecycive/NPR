namespace cfa.admin.context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class order_social_networks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stories", "OrderedBy", c => c.Long());
            CreateIndex("dbo.Stories","OrderedBy",false,"IX_StoryOrder");
            AddColumn("dbo.Stories", "FacebookTagLine", c => c.String(maxLength: 200));
            AddColumn("dbo.Stories", "FacebookBody", c => c.String());
            AddColumn("dbo.Stories", "TwitterTagLine", c => c.String(maxLength: 200));
            AddColumn("dbo.Stories", "LinkedInTagLine", c => c.String(maxLength: 200));
            AddColumn("dbo.Stories", "GooglePlusTagLine", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stories", "GooglePlusTagLine");
            DropColumn("dbo.Stories", "LinkedInTagLine");
            DropColumn("dbo.Stories", "TwitterTagLine");
            DropColumn("dbo.Stories", "FacebookBody");
            DropColumn("dbo.Stories", "FacebookTagLine");
            DropIndex("dbo.Stories", "IX_StoryOrder");
            DropColumn("dbo.Stories", "OrderedBy");
        }
    }
}
