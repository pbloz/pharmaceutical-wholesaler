namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isselected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IsSelected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "IsSelected");
        }
    }
}
