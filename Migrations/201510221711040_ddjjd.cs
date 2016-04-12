namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ddjjd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ManufactureId", c => c.Int(nullable: false));
            CreateIndex("dbo.Products", "ManufactureId");
            AddForeignKey("dbo.Products", "ManufactureId", "dbo.Manufactures", "ManufactureId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ManufactureId", "dbo.Manufactures");
            DropIndex("dbo.Products", new[] { "ManufactureId" });
            DropColumn("dbo.Products", "ManufactureId");
        }
    }
}
