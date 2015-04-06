namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "NewPersonUpdate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "NewPersonUpdate");
        }
    }
}
