namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class order : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Orders", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Orders", "UserId");
            RenameColumn(table: "dbo.Orders", name: "ApplicationUser_Id", newName: "UserId");
            AlterColumn("dbo.Orders", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Orders", "FirstName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Orders", "LastName", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Orders", "Address", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Orders", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Orders", "Email", c => c.String(nullable: false));
            CreateIndex("dbo.Orders", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Orders", new[] { "UserId" });
            AlterColumn("dbo.Orders", "Email", c => c.String());
            AlterColumn("dbo.Orders", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Orders", "Address", c => c.String());
            AlterColumn("dbo.Orders", "LastName", c => c.String(maxLength: 30));
            AlterColumn("dbo.Orders", "FirstName", c => c.String(maxLength: 30));
            AlterColumn("dbo.Orders", "UserId", c => c.String());
            RenameColumn(table: "dbo.Orders", name: "UserId", newName: "ApplicationUser_Id");
            AddColumn("dbo.Orders", "UserId", c => c.String());
            CreateIndex("dbo.Orders", "ApplicationUser_Id");
        }
    }
}
