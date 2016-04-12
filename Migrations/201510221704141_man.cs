namespace Hurtownia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class man : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Manufactures",
                c => new
                    {
                        ManufactureId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(maxLength: 100),
                        CodeAndCity = c.String(maxLength: 30),
                        PhoneNumber = c.String(),
                        Nip = c.String(),
                    })
                .PrimaryKey(t => t.ManufactureId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Manufactures");
        }
    }
}
