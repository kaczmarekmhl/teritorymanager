namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonDoNotVisit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "DoNotVisit", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "DoNotVisit");
        }
    }
}
