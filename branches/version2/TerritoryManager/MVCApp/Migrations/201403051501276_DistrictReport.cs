namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DistrictReport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DistrictReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        State = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        District_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Districts", t => t.District_Id, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.District_Id)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DistrictReports", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.DistrictReports", "District_Id", "dbo.Districts");
            DropIndex("dbo.DistrictReports", new[] { "UserId" });
            DropIndex("dbo.DistrictReports", new[] { "District_Id" });
            DropTable("dbo.DistrictReports");
        }
    }
}
