namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ddd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "Product_ProductId", "dbo.Products");
            DropIndex("dbo.Comments", new[] { "Product_ProductId" });
            RenameColumn(table: "dbo.Comments", name: "Product_ProductId", newName: "ProductId");
            AlterColumn("dbo.Comments", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.Comments", "ProductId");
            AddForeignKey("dbo.Comments", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "ProductId", "dbo.Products");
            DropIndex("dbo.Comments", new[] { "ProductId" });
            AlterColumn("dbo.Comments", "ProductId", c => c.Int());
            RenameColumn(table: "dbo.Comments", name: "ProductId", newName: "Product_ProductId");
            CreateIndex("dbo.Comments", "Product_ProductId");
            AddForeignKey("dbo.Comments", "Product_ProductId", "dbo.Products", "ProductId");
        }
    }
}
