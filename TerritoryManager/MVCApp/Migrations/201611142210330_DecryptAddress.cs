namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecryptAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Districts", "MigrationVersion", c => c.Int(nullable: false));
            AddColumn("dbo.People", "StreetAddress", c => c.String(maxLength: 45));
            AddColumn("dbo.People", "Longitude", c => c.String(maxLength: 15));
            AddColumn("dbo.People", "Latitude", c => c.String(maxLength: 15));
            AddColumn("dbo.People", "cx2", c => c.Binary());
            AddColumn("dbo.People", "MigrationVersion", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "MigrationVersion");
            DropColumn("dbo.People", "cx2");
            DropColumn("dbo.People", "Latitude");
            DropColumn("dbo.People", "Longitude");
            DropColumn("dbo.People", "StreetAddress");
            DropColumn("dbo.Districts", "MigrationVersion");
        }
    }
}
