namespace ATTP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class youtube : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigSites", "LinkYoutube", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfigSites", "LinkYoutube");
        }
    }
}
