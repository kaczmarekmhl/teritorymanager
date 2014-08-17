namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CongregationCountry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Congregations", "Country", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Congregations", "Country");
        }
    }
}
