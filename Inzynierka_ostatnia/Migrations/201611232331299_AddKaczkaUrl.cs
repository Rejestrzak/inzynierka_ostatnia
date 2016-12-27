namespace Inzynierka_ostatnia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKaczkaUrl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        username = c.String(),
                        password = c.String(),
                        type = c.Int(nullable: false),
                        created_at = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
