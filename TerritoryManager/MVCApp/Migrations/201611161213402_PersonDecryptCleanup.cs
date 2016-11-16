namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonDecryptCleanup : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Districts", "MigrationVersion");
            DropColumn("dbo.People", "cx");
            DropColumn("dbo.People", "MigrationVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.People", "MigrationVersion", c => c.Int(nullable: false));
            AddColumn("dbo.People", "cx", c => c.Binary());
            AddColumn("dbo.Districts", "MigrationVersion", c => c.Int(nullable: false));
        }
    }
}
