namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nip : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserData_Factory", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "UserData_Nip", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "UserData_FirstName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "UserData_LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "UserData_Address", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.AspNetUsers", "UserData_CodeAndCity", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.AspNetUsers", "UserData_Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "UserData_Email", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_CodeAndCity", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_Address", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_LastName", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserData_FirstName", c => c.String());
            DropColumn("dbo.AspNetUsers", "UserData_Nip");
            DropColumn("dbo.AspNetUsers", "UserData_Factory");
        }
    }
}
