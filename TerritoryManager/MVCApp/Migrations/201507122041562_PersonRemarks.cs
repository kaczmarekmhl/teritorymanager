namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonRemarks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "Remarks", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "Remarks");
        }
    }
}
