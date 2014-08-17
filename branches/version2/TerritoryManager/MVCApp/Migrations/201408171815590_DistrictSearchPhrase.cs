namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DistrictSearchPhrase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Districts", "SearchPhrase", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Districts", "SearchPhrase");
        }
    }
}
