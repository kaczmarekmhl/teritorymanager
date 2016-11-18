namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DistrictLastSearchUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Districts", "LastSearchUpdate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Districts", "LastSearchUpdate");
        }
    }
}
