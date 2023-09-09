namespace ATTP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class date : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigSites", "QR", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfigSites", "QR");
        }
    }
}
