namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menu_poly2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Occurrences", "Discriminator", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Occurrences", "Discriminator");
        }
    }
}
