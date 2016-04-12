namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateEndAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "DateEnd", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "DateEnd");
        }
    }
}
