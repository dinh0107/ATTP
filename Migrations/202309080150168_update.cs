namespace ATTP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductCategories", "ShowFooter", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductCategories", "ShowFooter");
        }
    }
}
