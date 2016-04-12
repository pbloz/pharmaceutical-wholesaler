namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "UserData_LastName", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_Address", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_CodeAndCity", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_Email", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "UserData_Email", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "UserData_CodeAndCity", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.AspNetUsers", "UserData_Address", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.AspNetUsers", "UserData_LastName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
