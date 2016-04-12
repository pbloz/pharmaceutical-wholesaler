namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodanieWalidacji : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "NameCategory", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Categories", "Description", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Products", "ProductName", c => c.String(nullable: false, maxLength: 40));
            AlterColumn("dbo.Products", "ProductDescription", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Orders", "Address", c => c.String(nullable: false, maxLength: 40));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "Address", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Products", "ProductDescription", c => c.String());
            AlterColumn("dbo.Products", "ProductName", c => c.String());
            AlterColumn("dbo.Categories", "Description", c => c.String());
            AlterColumn("dbo.Categories", "NameCategory", c => c.String());
        }
    }
}
