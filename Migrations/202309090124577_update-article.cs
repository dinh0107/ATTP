namespace ATTP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatearticle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "About", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "About");
        }
    }
}
