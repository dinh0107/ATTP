namespace ATTP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class configs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigSites", "NameBank", c => c.String(maxLength: 200));
            AddColumn("dbo.ConfigSites", "UseBank", c => c.String(maxLength: 200));
            AddColumn("dbo.ConfigSites", "NumberBank", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfigSites", "NumberBank");
            DropColumn("dbo.ConfigSites", "UseBank");
            DropColumn("dbo.ConfigSites", "NameBank");
        }
    }
}
