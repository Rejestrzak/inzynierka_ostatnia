namespace Inzynierka_ostatnia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUser_TypesTable1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.User_types",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        opis = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.User_types");
        }
    }
}
