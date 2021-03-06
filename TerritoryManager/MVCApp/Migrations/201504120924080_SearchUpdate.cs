namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "SearchUpdate", c => c.Boolean(nullable: false, defaultValue:true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "SearchUpdate");
        }
    }
}
