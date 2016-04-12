namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cxx : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BonusCodes",
                c => new
                    {
                        BonusCodeId = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => t.BonusCodeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BonusCodes");
        }
    }
}
