namespace Inzynierka_ostatnia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Kaczkas",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        id_matki = c.Int(nullable: false),
                        id_ojca = c.Int(nullable: false),
                        wyklucie = c.DateTime(nullable: false),
                        zyje = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Kaczkas");
        }
    }
}
