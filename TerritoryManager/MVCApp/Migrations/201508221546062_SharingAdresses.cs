namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SharingAdresses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Congregations", "SharingAddressesEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Congregations", "SharingAddressesEnabled");
        }
    }
}
