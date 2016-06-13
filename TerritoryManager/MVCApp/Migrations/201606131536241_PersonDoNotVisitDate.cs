namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonDoNotVisitDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "DoNotVisitReportDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "DoNotVisitReportDate");
        }
    }
}
