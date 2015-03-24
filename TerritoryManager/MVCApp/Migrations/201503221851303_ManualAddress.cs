namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManualAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "Manual", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "Manual");
        }
    }
}
