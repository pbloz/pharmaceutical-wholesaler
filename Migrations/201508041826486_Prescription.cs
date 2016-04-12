namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Prescription", c => c.Boolean(nullable: false));
            DropColumn("dbo.Products", "IsSelected");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "IsSelected", c => c.Boolean(nullable: false));
            DropColumn("dbo.Products", "Prescription");
        }
    }
}
