namespace Inzynierka_ostatnia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnZmiana : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Kaczkas", "zmiana", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Kaczkas", "zmiana");
        }
    }
}
