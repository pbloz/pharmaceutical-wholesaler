namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class quadro : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Form", c => c.String());
            AddColumn("dbo.Products", "Components", c => c.String());
            AddColumn("dbo.Products", "Contraindications", c => c.String());
            AlterColumn("dbo.Categories", "NameCategory", c => c.String(nullable: false));
            AlterColumn("dbo.Categories", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "ProductName", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "ProductDescription", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "UserData_FirstName", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_LastName", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_Address", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_CodeAndCity", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "UserData_CodeAndCity", c => c.String(maxLength: 30));
            AlterColumn("dbo.AspNetUsers", "UserData_Address", c => c.String(maxLength: 100));
            AlterColumn("dbo.AspNetUsers", "UserData_LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "UserData_FirstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Products", "ProductDescription", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Products", "ProductName", c => c.String(nullable: false, maxLength: 40));
            AlterColumn("dbo.Categories", "Description", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Categories", "NameCategory", c => c.String(nullable: false, maxLength: 20));
            DropColumn("dbo.Products", "Contraindications");
            DropColumn("dbo.Products", "Components");
            DropColumn("dbo.Products", "Form");
        }
    }
}
