namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonIsVisitedByOtherPublisher : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "IsVisitedByOtherPublisher", c => c.Boolean(nullable: false));
            AddColumn("dbo.People", "VisitingPublisher", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "VisitingPublisher");
            DropColumn("dbo.People", "IsVisitedByOtherPublisher");
        }
    }
}
