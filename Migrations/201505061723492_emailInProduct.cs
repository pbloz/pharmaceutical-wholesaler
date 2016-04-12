namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailInProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Email");
        }
    }
}
