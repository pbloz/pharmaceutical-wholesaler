namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zmianas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Factory", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "Nip", c => c.String(nullable: false));
            AlterColumn("dbo.Orders", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Orders", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Orders", "Address", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Orders", "CodeAndCity", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Orders", "PhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Orders", "CodeAndCity", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Orders", "Address", c => c.String(nullable: false, maxLength: 40));
            AlterColumn("dbo.Orders", "LastName", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Orders", "FirstName", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Orders", "Nip");
            DropColumn("dbo.Orders", "Factory");
        }
    }
}
