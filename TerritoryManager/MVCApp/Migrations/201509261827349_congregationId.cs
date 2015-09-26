namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class congregationId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.UserProfile", name: "Congregation_Id", newName: "CongregationId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.UserProfile", name: "CongregationId", newName: "Congregation_Id");
        }
    }
}
