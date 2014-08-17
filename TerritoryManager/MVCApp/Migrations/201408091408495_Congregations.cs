namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Congregations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Congregations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 45),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Districts", "Congregation_Id", c => c.Int());
            AddColumn("dbo.UserProfile", "Congregation_Id", c => c.Int());
            CreateIndex("dbo.UserProfile", "Congregation_Id");
            CreateIndex("dbo.Districts", "Congregation_Id");
            AddForeignKey("dbo.UserProfile", "Congregation_Id", "dbo.Congregations", "Id");
            AddForeignKey("dbo.Districts", "Congregation_Id", "dbo.Congregations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Districts", "Congregation_Id", "dbo.Congregations");
            DropForeignKey("dbo.UserProfile", "Congregation_Id", "dbo.Congregations");
            DropIndex("dbo.Districts", new[] { "Congregation_Id" });
            DropIndex("dbo.UserProfile", new[] { "Congregation_Id" });
            DropColumn("dbo.UserProfile", "Congregation_Id");
            DropColumn("dbo.Districts", "Congregation_Id");
            DropTable("dbo.Congregations");
        }
    }
}
