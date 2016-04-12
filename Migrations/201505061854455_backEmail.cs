namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class backEmail : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Products", "Email");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Email", c => c.String());
        }
    }
}
