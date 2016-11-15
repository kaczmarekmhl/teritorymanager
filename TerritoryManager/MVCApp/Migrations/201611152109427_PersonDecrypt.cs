namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonDecrypt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Districts", "MigrationVersion", c => c.Int(nullable: false));
            AddColumn("dbo.People", "Lastname", c => c.String(maxLength: 45));
            AddColumn("dbo.People", "StreetAddress", c => c.String(maxLength: 45));
            AddColumn("dbo.People", "TelephoneNumber", c => c.String(maxLength: 30));
            AddColumn("dbo.People", "Longitude", c => c.String(maxLength: 15));
            AddColumn("dbo.People", "Latitude", c => c.String(maxLength: 15));
            AddColumn("dbo.People", "MigrationVersion", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "MigrationVersion");
            DropColumn("dbo.People", "Latitude");
            DropColumn("dbo.People", "Longitude");
            DropColumn("dbo.People", "TelephoneNumber");
            DropColumn("dbo.People", "StreetAddress");
            DropColumn("dbo.People", "Lastname");
            DropColumn("dbo.Districts", "MigrationVersion");
        }
    }
}
