namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pcid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "ProductId", "dbo.Products");
            DropIndex("dbo.Comments", new[] { "ProductId" });
            RenameColumn(table: "dbo.Comments", name: "ProductId", newName: "Product_ProductId");
            AddColumn("dbo.Comments", "ProductComId", c => c.Int(nullable: false));
            AlterColumn("dbo.Comments", "Product_ProductId", c => c.Int());
            CreateIndex("dbo.Comments", "Product_ProductId");
            AddForeignKey("dbo.Comments", "Product_ProductId", "dbo.Products", "ProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "Product_ProductId", "dbo.Products");
            DropIndex("dbo.Comments", new[] { "Product_ProductId" });
            AlterColumn("dbo.Comments", "Product_ProductId", c => c.Int(nullable: false));
            DropColumn("dbo.Comments", "ProductComId");
            RenameColumn(table: "dbo.Comments", name: "Product_ProductId", newName: "ProductId");
            CreateIndex("dbo.Comments", "ProductId");
            AddForeignKey("dbo.Comments", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
    }
}
